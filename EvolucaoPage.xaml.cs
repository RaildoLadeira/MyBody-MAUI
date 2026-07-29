using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MyBody;

public class RegistroPeso
{
    public string Data { get; set; } = string.Empty;
    public double Peso { get; set; }
}

public partial class EvolucaoPage : ContentPage
{
    private const string PREF_KEY_PESO_HIST = "UserHistoricoPeso_v1";
    private const string PREF_KEY_FOTOS = "UserFotosProgresso_v1";

    private List<RegistroPeso> _historicoPeso = new();
    private List<string> _caminhosFotos = new();

    public EvolucaoPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CarregarDados();
        CalcularMetricasCorporais();
        AtualizarTextosIdioma();
    }

    private void CarregarDados()
    {
        // Carrega histórico de pesos
        string jsonPeso = Preferences.Get(PREF_KEY_PESO_HIST, string.Empty);
        if (!string.IsNullOrWhiteSpace(jsonPeso))
        {
            try { _historicoPeso = JsonSerializer.Deserialize<List<RegistroPeso>>(jsonPeso) ?? new(); } catch { }
        }

        // Carrega caminhos das fotos de progresso
        string jsonFotos = Preferences.Get(PREF_KEY_FOTOS, string.Empty);
        if (!string.IsNullOrWhiteSpace(jsonFotos))
        {
            try { _caminhosFotos = JsonSerializer.Deserialize<List<string>>(jsonFotos) ?? new(); } catch { }
        }

        AtualizarHistoricoPesoVisual();
        AtualizarGaleriaFotosVisual();
    }

    private void CalcularMetricasCorporais()
    {
        double.TryParse(Preferences.Get("UserPeso", string.Empty), out double peso);
        double.TryParse(Preferences.Get("UserAltura", string.Empty), out double alturaCm);
        int.TryParse(Preferences.Get("UserIdade", string.Empty), out int idade);
        int sexoIdx = Preferences.Get("UserSexoIndex", 0); // 0=Masculino, 1=Feminino
        int ativIdx = Preferences.Get("UserAtividadeIndex", 0);

        if (peso <= 0 || alturaCm <= 0)
        {
            lblIMC.Text = "IMC: Informe peso e altura na aba Dieta";
            lblTMB.Text = "TMB: --";
            lblGastoTotal.Text = "Gasto Energético: --";
            return;
        }

        // 1. Cálculo do IMC = Peso / (Altura em Metros)^2
        double alturaM = alturaCm / 100.0;
        double imc = peso / (alturaM * alturaM);
        string classificacao = ObterClassificacaoIMC(imc);

        string formatoIMC = LocalizationService.Get("ResultadoIMC");
        lblIMC.Text = string.Format(formatoIMC, imc, classificacao);

        // 2. Cálculo da TMB (Fórmula de Harris-Benedict)
        double tmb;
        if (sexoIdx == 0) // Masculino
            tmb = 88.36 + (13.4 * peso) + (4.8 * alturaCm) - (5.7 * (idade > 0 ? idade : 25));
        else // Feminino
            tmb = 447.59 + (9.24 * peso) + (3.1 * alturaCm) - (4.33 * (idade > 0 ? idade : 25));

        string formatoTMB = LocalizationService.Get("ResultadoTMB");
        lblTMB.Text = string.Format(formatoTMB, tmb);

        // 3. Gasto Energético Total
        double fatorAtividade = ativIdx switch
        {
            1 => 1.375, // Leve
            2 => 1.55,  // Moderado
            3 => 1.725, // Muito Ativo
            _ => 1.2    // Sedentário
        };

        double gastoTotal = tmb * fatorAtividade;
        string formatoGasto = LocalizationService.Get("GastoTotal");
        lblGastoTotal.Text = string.Format(formatoGasto, gastoTotal);
    }

    private string ObterClassificacaoIMC(double imc)
    {
        if (imc < 18.5) return "Abaixo do peso";
        if (imc < 24.9) return "Peso normal";
        if (imc < 29.9) return "Sobrepeso";
        return "Obesidade";
    }

    private async void OnRegistrarPesoClicked(object sender, EventArgs e)
    {
        string pesoAtual = Preferences.Get("UserPeso", string.Empty);
        string resultado = await DisplayPromptAsync("Registrar Peso", "Confirme ou digite seu peso de hoje (kg):", initialValue: pesoAtual, keyboard: Keyboard.Numeric);

        if (double.TryParse(resultado, out double novoPeso) && novoPeso > 0)
        {
            Preferences.Set("UserPeso", novoPeso.ToString());
            _historicoPeso.Insert(0, new RegistroPeso { Data = DateTime.Now.ToString("dd/MM/yyyy"), Peso = novoPeso });

            Preferences.Set(PREF_KEY_PESO_HIST, JsonSerializer.Serialize(_historicoPeso));

            CalcularMetricasCorporais();
            AtualizarHistoricoPesoVisual();
        }
    }

    private void AtualizarHistoricoPesoVisual()
    {
        containerHistoricoPeso.Children.Clear();

        if (_historicoPeso.Count == 0)
        {
            containerHistoricoPeso.Children.Add(new Label { Text = "Nenhum histórico gravado.", TextColor = Colors.Gray, FontSize = 13 });
            return;
        }

        foreach (var item in _historicoPeso.Take(5))
        {
            containerHistoricoPeso.Children.Add(new Label
            {
                Text = $"• {item.Data} — {item.Peso:F1} kg",
                FontSize = 14,
                TextColor = Colors.DarkSlateGray
            });
        }
    }

    private async void OnSelecionarFotoClicked(object sender, EventArgs e)
    {
        try
        {
            var foto = await MediaPicker.Default.PickPhotoAsync();
            if (foto != null)
            {
                string caminhoLocal = Path.Combine(FileSystem.AppDataDirectory, foto.FileName);
                using (var stream = await foto.OpenReadAsync())
                using (var newStream = File.Create(caminhoLocal))
                {
                    await stream.CopyToAsync(newStream);
                }

                _caminhosFotos.Insert(0, caminhoLocal);
                Preferences.Set(PREF_KEY_FOTOS, JsonSerializer.Serialize(_caminhosFotos));
                AtualizarGaleriaFotosVisual();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar a imagem: {ex.Message}", "OK");
        }
    }

    private void AtualizarGaleriaFotosVisual()
    {
        containerFotos.Children.Clear();

        if (_caminhosFotos.Count == 0)
        {
            containerFotos.Children.Add(new Label { Text = "Nenhuma foto adicionada ainda.", TextColor = Colors.Gray, FontSize = 13 });
            return;
        }

        foreach (var caminho in _caminhosFotos)
        {
            var img = new Image
            {
                Source = ImageSource.FromFile(caminho),
                WidthRequest = 100,
                HeightRequest = 140,
                Aspect = Aspect.AspectFill
            };

            var border = new Border
            {
                Content = img,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Margin = new Thickness(0, 0, 5, 0)
            };

            containerFotos.Children.Add(border);
        }
    }

    private void AtualizarTextosIdioma()
    {
        Title = LocalizationService.Get("TabEvolucao");

        if (lblTituloEvolucao == null) return;

        lblTituloEvolucao.Text = LocalizationService.Get("TituloEvolucao");
        lblHistoricoPeso.Text = LocalizationService.Get("HistoricoPeso");
        btnRegistrarPeso.Text = LocalizationService.Get("RegistrarNovoPeso");
        lblFotosEvolucao.Text = LocalizationService.Get("FotosEvolucao");
        btnSelecionarFoto.Text = LocalizationService.Get("SelecionarFoto");

        CalcularMetricasCorporais();
    }
}