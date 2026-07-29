using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBody;

public class ItemAlimento
{
    public string ChaveTraducao { get; set; } = string.Empty;
    public double CaloriasPorUnidadeMedida { get; set; }
    public bool EhPorUnidade { get; set; }
}

public partial class MainPage : ContentPage
{
    private readonly List<ItemAlimento> _tabelaAlimentos = new();
    private double _totalCaloriasHoje = 0;

    public MainPage()
    {
        InitializeComponent();
        InicializarTabelaAlimentos();
        CarregarDadosSalvos();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarTextosIdioma();

        if (Shell.Current is AppShell appShell)
        {
            appShell.AtualizarTitulosAbas();
        }
    }

    private void OnAlternarTemaClicked(object sender, EventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Dark
                ? AppTheme.Light
                : AppTheme.Dark;
        }
    }

    private void InicializarTabelaAlimentos()
    {
        _tabelaAlimentos.Clear();

        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_OvoCozido", CaloriasPorUnidadeMedida = 72, EhPorUnidade = true });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_OvoMexido", CaloriasPorUnidadeMedida = 85, EhPorUnidade = true });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_FrangoGrelhado", CaloriasPorUnidadeMedida = 1.65, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_ArrozBranco", CaloriasPorUnidadeMedida = 1.30, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_ArrozIntegral", CaloriasPorUnidadeMedida = 1.11, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_FeijaoPreto", CaloriasPorUnidadeMedida = 0.77, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_BatataDoce", CaloriasPorUnidadeMedida = 0.86, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_Banana", CaloriasPorUnidadeMedida = 0.89, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_Maca", CaloriasPorUnidadeMedida = 0.52, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_Aveia", CaloriasPorUnidadeMedida = 3.89, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_Whey", CaloriasPorUnidadeMedida = 4.00, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_LeiteAveia", CaloriasPorUnidadeMedida = 0.45, EhPorUnidade = false });
        _tabelaAlimentos.Add(new ItemAlimento { ChaveTraducao = "Alimento_CafeEspresso", CaloriasPorUnidadeMedida = 0.02, EhPorUnidade = false });

        AtualizarListaAlimentosPicker();
    }

    private void AtualizarListaAlimentosPicker()
    {
        if (pickerAlimentos == null) return;

        int indexAtual = pickerAlimentos.SelectedIndex < 0 ? 0 : pickerAlimentos.SelectedIndex;

        pickerAlimentos.ItemsSource = _tabelaAlimentos
            .Select(a => LocalizationService.Get(a.ChaveTraducao))
            .ToList();

        if (pickerAlimentos.ItemsSource.Count > 0)
        {
            pickerAlimentos.SelectedIndex = indexAtual;
        }
    }

    private void OnAlimentoSelected(object sender, EventArgs e)
    {
        if (pickerAlimentos.SelectedIndex < 0 || pickerAlimentos.SelectedIndex >= _tabelaAlimentos.Count) return;

        var alimentoSelecionado = _tabelaAlimentos[pickerAlimentos.SelectedIndex];

        if (alimentoSelecionado.EhPorUnidade)
        {
            lblQtdGramas.Text = LocalizationService.Get("QtdUnidades");
            txtQuantidade.Placeholder = "Ex: 2";
        }
        else
        {
            lblQtdGramas.Text = LocalizationService.Get("QtdGramas");
            txtQuantidade.Placeholder = "Ex: 100";
        }
    }

    private void CarregarDadosSalvos()
    {
        txtPeso.Text = Preferences.Get("UserPeso", string.Empty);
        txtAltura.Text = Preferences.Get("UserAltura", string.Empty);
        txtIdade.Text = Preferences.Get("UserIdade", string.Empty);
        pickerSexo.SelectedIndex = Preferences.Get("UserSexoIndex", 0);
        pickerAtividade.SelectedIndex = Preferences.Get("UserAtividadeIndex", 0);

        CalcularMetaAgua();
    }

    private void CalcularMetaAgua()
    {
        if (double.TryParse(txtPeso.Text, out double peso) && peso > 0)
        {
            double metaMilitros = peso * 35;
            double metaLitros = metaMilitros / 1000.0;

            string formato = LocalizationService.Get("MetaAguaFormat");
            lblMetaAgua.Text = string.Format(formato, metaLitros, metaMilitros);
        }
        else
        {
            lblMetaAgua.Text = LocalizationService.Get("InformePeso");
        }
    }

    private async void OnSalvarDadosClicked(object sender, EventArgs e)
    {
        Preferences.Set("UserPeso", txtPeso.Text ?? string.Empty);
        Preferences.Set("UserAltura", txtAltura.Text ?? string.Empty);
        Preferences.Set("UserIdade", txtIdade.Text ?? string.Empty);
        Preferences.Set("UserSexoIndex", pickerSexo.SelectedIndex);
        Preferences.Set("UserAtividadeIndex", pickerAtividade.SelectedIndex);

        CalcularMetaAgua();

        await DisplayAlert("Sucesso", "Informações e metas salvas com sucesso!", "OK");
    }

    private async void OnZerarHojeClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Confirmar", "Deseja zerar os registros de hoje?", "Sim", "Não");
        if (confirmar)
        {
            _totalCaloriasHoje = 0;
            AtualizarTotalCaloriasLabel();
            containerRefeicoes.Children.Clear();
            await DisplayAlert("Pronto", "Registros de hoje foram zerados.", "OK");
        }
    }

    private async void OnAdicionarRefeicaoClicked(object sender, EventArgs e)
    {
        if (pickerAlimentos.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtQuantidade.Text))
        {
            await DisplayAlert("Atenção", "Selecione um alimento e informe a quantidade.", "OK");
            return;
        }

        if (!double.TryParse(txtQuantidade.Text, out double quantidade) || quantidade <= 0)
        {
            await DisplayAlert("Atenção", "Informe um valor numérico válido.", "OK");
            return;
        }

        var alimento = _tabelaAlimentos[pickerAlimentos.SelectedIndex];
        string nomeTraduzido = LocalizationService.Get(alimento.ChaveTraducao);

        double caloriasItem = quantidade * alimento.CaloriasPorUnidadeMedida;
        _totalCaloriasHoje += caloriasItem;

        string unidadeTexto = alimento.EhPorUnidade ? "un" : "g";
        string momento = pickerMomento.SelectedItem?.ToString() ?? "Refeição";

        string itemTexto = $"• {momento}: {nomeTraduzido} ({quantidade}{unidadeTexto}) - {caloriasItem:F0} kcal";

        var lblItem = new Label
        {
            Text = itemTexto,
            FontSize = 14,
            TextColor = Colors.DarkSlateGray
        };

        containerRefeicoes.Children.Add(lblItem);
        AtualizarTotalCaloriasLabel();

        txtQuantidade.Text = string.Empty;
    }

    private void AtualizarTotalCaloriasLabel()
    {
        string formatoCalorias = LocalizationService.Get("TotalCalorias");
        lblTotalCalorias.Text = string.Format(formatoCalorias, _totalCaloriasHoje);
    }

    private void OnIdiomaChanged(object sender, EventArgs e)
    {
        if (pickerIdioma == null) return;

        LocalizationService.IdiomaAtual = pickerIdioma.SelectedIndex switch
        {
            1 => "en",
            2 => "es",
            _ => "pt"
        };

        AtualizarTextosIdioma();

        if (Shell.Current is AppShell appShell)
        {
            appShell.AtualizarTitulosAbas();
        }
    }

    private void AtualizarTextosIdioma()
    {
        Title = LocalizationService.Get("TabDieta");

        if (lblDadosPessoais == null) return;

        lblDadosPessoais.Text = LocalizationService.Get("DadosPessoais");
        lblPeso.Text = LocalizationService.Get("Peso");
        lblAltura.Text = LocalizationService.Get("Altura");
        lblIdade.Text = LocalizationService.Get("Idade");
        lblSexo.Text = LocalizationService.Get("Sexo");
        lblAtividade.Text = LocalizationService.Get("Atividade");
        btnCalcularSalvar.Text = LocalizationService.Get("CalcularSalvar");

        lblHidratacao.Text = LocalizationService.Get("Hidratacao");
        lblCalcPeso.Text = LocalizationService.Get("CalcPeso");
        btnZerarHoje.Text = LocalizationService.Get("ZerarHoje");

        lblConsumoCalorico.Text = LocalizationService.Get("ConsumoCalorico");
        lblRefeicoesDia.Text = LocalizationService.Get("RefeicoesDia");
        lblCadastrarRefeicao.Text = LocalizationService.Get("CadastrarRefeicao");
        lblMomentoRefeicao.Text = LocalizationService.Get("MomentoRefeicao");
        lblSelecioneAlimento.Text = LocalizationService.Get("SelecioneAlimento");
        btnAdicionarRefeicao.Text = LocalizationService.Get("AdicionarRefeicao");

        AtualizarListaAlimentosPicker();
        OnAlimentoSelected(this, EventArgs.Empty);
        AtualizarPickersPorIdioma();
        CalcularMetaAgua();
        AtualizarTotalCaloriasLabel();
    }

    private void AtualizarPickersPorIdioma()
    {
        int idxMomento = pickerMomento.SelectedIndex < 0 ? 0 : pickerMomento.SelectedIndex;
        int idxSexo = pickerSexo.SelectedIndex < 0 ? 0 : pickerSexo.SelectedIndex;
        int idxAtiv = pickerAtividade.SelectedIndex < 0 ? 0 : pickerAtividade.SelectedIndex;

        switch (LocalizationService.IdiomaAtual)
        {
            case "en":
                pickerMomento.ItemsSource = new List<string> { "Breakfast", "Lunch", "Afternoon Snack", "Dinner", "Supper / Snack" };
                pickerSexo.ItemsSource = new List<string> { "Male", "Female" };
                pickerAtividade.ItemsSource = new List<string> { "Sedentary", "Lightly Active", "Moderately Active", "Very Active" };
                break;
            case "es":
                pickerMomento.ItemsSource = new List<string> { "Desayuno", "Almuerzo", "Merienda", "Cena", "Tentempié" };
                pickerSexo.ItemsSource = new List<string> { "Masculino", "Femenino" };
                pickerAtividade.ItemsSource = new List<string> { "Sedentario", "Ligeramente Activo", "Moderadamente Activo", "Muy Activo" };
                break;
            default:
                pickerMomento.ItemsSource = new List<string> { "Café da Manhã", "Almoço", "Lanche da Tarde", "Jantar", "Ceia / Snaking" };
                pickerSexo.ItemsSource = new List<string> { "Masculino", "Feminino" };
                pickerAtividade.ItemsSource = new List<string> { "Sedentário", "Levemente Ativo", "Moderadamente Ativo", "Muito Ativo" };
                break;
        }

        pickerMomento.SelectedIndex = idxMomento;
        pickerSexo.SelectedIndex = idxSexo;
        pickerAtividade.SelectedIndex = idxAtiv;
    }
}