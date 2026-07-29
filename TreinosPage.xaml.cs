using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MyBody;

public class ItemExercicio
{
    public int Id { get; set; }
    public int FichaIndex { get; set; }
    public string GrupoMuscularKey { get; set; } = string.Empty;
    public string ChaveTraducaoNome { get; set; } = string.Empty;
    public string NomeCustomizado { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string Reps { get; set; } = string.Empty;
    public string Carga { get; set; } = string.Empty;
    public bool Concluido { get; set; }
}

public partial class TreinosPage : ContentPage
{
    private const string PREF_KEY_TREINOS = "UserExerciciosSalvos_v2";
    private List<ItemExercicio> _todosExercicios = new();
    private int _nextId = 1;

    public TreinosPage()
    {
        InitializeComponent();
        CarregarOuInicializarExercicios();
        pickerDiaFicha.SelectedIndex = 0;
        pickerGrupoMuscular.SelectedIndex = 0;
        AtualizarListaExerciciosPorGrupo();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CarregarOuInicializarExercicios();
        AtualizarTextosIdioma();
    }

    private void CarregarOuInicializarExercicios()
    {
        string jsonSalvo = Preferences.Get(PREF_KEY_TREINOS, string.Empty);

        if (!string.IsNullOrWhiteSpace(jsonSalvo))
        {
            try
            {
                var listaDeserializada = JsonSerializer.Deserialize<List<ItemExercicio>>(jsonSalvo);
                if (listaDeserializada != null)
                {
                    _todosExercicios = listaDeserializada;
                    _nextId = _todosExercicios.Count > 0 ? _todosExercicios.Max(e => e.Id) + 1 : 1;
                    return;
                }
            }
            catch { }
        }

        CarregarTreinosPadrao();
        SalvarExerciciosNoStorage();
    }

    private void SalvarExerciciosNoStorage()
    {
        try
        {
            string json = JsonSerializer.Serialize(_todosExercicios);
            Preferences.Set(PREF_KEY_TREINOS, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao salvar treinos: {ex.Message}");
        }
    }

    private void CarregarTreinosPadrao()
    {
        _todosExercicios.Clear();
        _nextId = 1;

        AdicionarExercicioPadrao(0, "Chest", "Ex_SupinoReto", "4", "10", "30");
        AdicionarExercicioPadrao(0, "Chest", "Ex_SupinoInclinado", "3", "12", "22");
        AdicionarExercicioPadrao(0, "Chest", "Ex_Crossover", "3", "15", "15");
        AdicionarExercicioPadrao(0, "Triceps", "Ex_TricepsPulley", "4", "12", "25");
        AdicionarExercicioPadrao(0, "Triceps", "Ex_TricepsTesta", "3", "10", "12");

        AdicionarExercicioPadrao(1, "Back", "Ex_PuxadaFrontal", "4", "10", "45");
        AdicionarExercicioPadrao(1, "Back", "Ex_RemadaCurvada", "4", "10", "35");
        AdicionarExercicioPadrao(1, "Back", "Ex_RemadaBaixa", "3", "12", "40");
        AdicionarExercicioPadrao(1, "Biceps", "Ex_RoscaDireta", "4", "10", "14");
        AdicionarExercicioPadrao(1, "Biceps", "Ex_RoscaMartelo", "3", "12", "12");

        AdicionarExercicioPadrao(2, "Legs", "Ex_Agachamento", "4", "10", "50");
        AdicionarExercicioPadrao(2, "Legs", "Ex_LegPress", "4", "12", "120");
        AdicionarExercicioPadrao(2, "Legs", "Ex_Extensora", "3", "15", "40");
        AdicionarExercicioPadrao(2, "Hamstrings", "Ex_Flexora", "4", "12", "35");
        AdicionarExercicioPadrao(2, "Calves", "Ex_Panturrilha", "4", "15", "50");

        AdicionarExercicioPadrao(3, "Shoulders", "Ex_Desenvolvimento", "4", "10", "18");
        AdicionarExercicioPadrao(3, "Shoulders", "Ex_ElevacaoLateral", "4", "12", "10");
        AdicionarExercicioPadrao(3, "Shoulders", "Ex_ElevacaoFrontal", "3", "12", "10");
        AdicionarExercicioPadrao(3, "Abs", "Ex_AbdominalInfra", "3", "15", "0");
        AdicionarExercicioPadrao(3, "Abs", "Ex_Prancha", "3", "45s", "0");

        AdicionarExercicioPadrao(4, "Cardio", "Ex_Esteira", "1", "30min", "0");
        AdicionarExercicioPadrao(4, "General", "Ex_Burpees", "3", "15", "0");
        AdicionarExercicioPadrao(4, "General", "Ex_Kettlebell", "4", "15", "16");
    }

    private void AdicionarExercicioPadrao(int fichaIdx, string grupoKey, string chaveNome, string series, string reps, string carga)
    {
        _todosExercicios.Add(new ItemExercicio
        {
            Id = _nextId++,
            FichaIndex = fichaIdx,
            GrupoMuscularKey = grupoKey,
            ChaveTraducaoNome = chaveNome,
            Series = series,
            Reps = reps,
            Carga = carga,
            Concluido = false
        });
    }

    private void OnGrupoMuscularChanged(object sender, EventArgs e)
    {
        AtualizarListaExerciciosPorGrupo();
    }

    private void AtualizarListaExerciciosPorGrupo()
    {
        if (pickerNomeExercicio == null) return;

        int grupoIndex = pickerGrupoMuscular.SelectedIndex;

        List<string> exerciciosSugestao = grupoIndex switch
        {
            0 => new() { "Supino Reto com Barra", "Supino Inclinado com Halteres", "Crucifixo Reto", "Crossover na Polia", "Peck Deck / Voador", "Outro (Digitar Manualmente)" },
            1 => new() { "Puxada Frontal", "Remada Curvada com Barra", "Remada Baixa Triângulo", "Pulldown na Polia", "Puxada Alta Articulada", "Outro (Digitar Manualmente)" },
            2 => new() { "Agachamento Livre", "Leg Press 45°", "Cadeira Extensora", "Afundo com Halteres", "Hack Squat", "Outro (Digitar Manualmente)" },
            3 => new() { "Mesa Flexora", "Cadeira Flexora", "Stiff com Barra", "Elevação Pélvica", "Outro (Digitar Manualmente)" },
            4 => new() { "Desenvolvimento com Halteres", "Elevação Lateral", "Elevação Frontal", "Crucifixo Invertido", "Outro (Digitar Manualmente)" },
            5 => new() { "Rosca Direta W", "Rosca Martelo", "Rosca Concentrada", "Rosca Scott", "Outro (Digitar Manualmente)" },
            6 => new() { "Tríceps Pulley", "Tríceps Testa", "Tríceps Corda", "Tríceps Coice", "Outro (Digitar Manualmente)" },
            7 => new() { "Gêmeos em Pé", "Gêmeos Sentado", "Panturrilha no Leg Press", "Outro (Digitar Manualmente)" },
            8 => new() { "Abdominal Infra", "Abdominal Supra", "Prancha Ventral", "Abdominal na Polia", "Outro (Digitar Manualmente)" },
            _ => new() { "Outro (Digitar Manualmente)" }
        };

        pickerNomeExercicio.ItemsSource = exerciciosSugestao;
        pickerNomeExercicio.SelectedIndex = 0;
    }

    private void OnNomeExercicioSelected(object sender, EventArgs e)
    {
        if (pickerNomeExercicio.SelectedItem == null) return;

        string selecionado = pickerNomeExercicio.SelectedItem.ToString() ?? string.Empty;
        txtNomeExercicioCustom.IsVisible = selecionado.StartsWith("Outro");
    }

    private void OnDiaFichaChanged(object sender, EventArgs e)
    {
        AtualizarListaVisual();
    }

    private void AtualizarListaVisual()
    {
        if (containerExerciciosFicha == null) return;

        containerExerciciosFicha.Children.Clear();

        int fichaSelecionadaIndex = pickerDiaFicha.SelectedIndex < 0 ? 0 : pickerDiaFicha.SelectedIndex;

        var exerciciosDoDia = _todosExercicios
            .Where(e => e.FichaIndex == fichaSelecionadaIndex)
            .ToList();

        if (exerciciosDoDia.Count == 0)
        {
            containerExerciciosFicha.Children.Add(new Label
            {
                Text = LocalizationService.Get("SemExercicios"),
                TextColor = Colors.Gray,
                FontSize = 14
            });
            return;
        }

        foreach (var item in exerciciosDoDia)
        {
            string nomeExercicio = string.IsNullOrEmpty(item.NomeCustomizado)
                ? LocalizationService.Get(item.ChaveTraducaoNome)
                : item.NomeCustomizado;

            var cardItem = new Border
            {
                Padding = 10,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                BackgroundColor = item.Concluido ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#F8F9FA")
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var checkConcluido = new CheckBox
            {
                IsChecked = item.Concluido,
                Color = Colors.Green,
                VerticalOptions = LayoutOptions.Center
            };

            var infoStack = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            var lblNome = new Label
            {
                Text = $"• [{item.GrupoMuscularKey}] {nomeExercicio}",
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                TextColor = item.Concluido ? Colors.Gray : Color.FromArgb("#333333"),
                TextDecorations = item.Concluido ? TextDecorations.Strikethrough : TextDecorations.None
            };

            var lblDetalhes = new Label
            {
                Text = $"{item.Series} {LocalizationService.Get("Series")} x {item.Reps} {LocalizationService.Get("Reps")} | {LocalizationService.Get("Carga")}: {item.Carga} kg",
                FontSize = 13,
                TextColor = item.Concluido ? Colors.Gray : Color.FromArgb("#555555")
            };

            checkConcluido.CheckedChanged += (s, args) =>
            {
                item.Concluido = args.Value;
                SalvarExerciciosNoStorage();
                AtualizarListaVisual();
            };

            infoStack.Children.Add(lblNome);
            infoStack.Children.Add(lblDetalhes);

            var btnRemover = new Button
            {
                Text = "❌",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Red,
                FontSize = 12,
                Padding = 2,
                VerticalOptions = LayoutOptions.Center
            };

            btnRemover.Clicked += (s, args) =>
            {
                _todosExercicios.RemoveAll(x => x.Id == item.Id);
                SalvarExerciciosNoStorage();
                AtualizarListaVisual();
            };

            Grid.SetColumn(checkConcluido, 0);
            Grid.SetColumn(infoStack, 1);
            Grid.SetColumn(btnRemover, 2);

            grid.Children.Add(checkConcluido);
            grid.Children.Add(infoStack);
            grid.Children.Add(btnRemover);

            cardItem.Content = grid;
            containerExerciciosFicha.Children.Add(cardItem);
        }
    }

    private async void OnZerarChecklistClicked(object sender, EventArgs e)
    {
        int fichaSelecionadaIndex = pickerDiaFicha.SelectedIndex < 0 ? 0 : pickerDiaFicha.SelectedIndex;

        var exerciciosDoDia = _todosExercicios
            .Where(x => x.FichaIndex == fichaSelecionadaIndex)
            .ToList();

        if (exerciciosDoDia.Count == 0) return;

        bool confirmar = await DisplayAlert("Resetar Treino", "Deseja desmarcar todos os exercícios deste treino?", "Sim", "Não");
        if (confirmar)
        {
            foreach (var item in exerciciosDoDia)
            {
                item.Concluido = false;
            }

            SalvarExerciciosNoStorage();
            AtualizarListaVisual();
        }
    }

    private async void OnSalvarExercicioClicked(object sender, EventArgs e)
    {
        string nomeFinal = string.Empty;

        if (pickerNomeExercicio.SelectedItem != null)
        {
            string selecionado = pickerNomeExercicio.SelectedItem.ToString() ?? string.Empty;

            if (selecionado.StartsWith("Outro"))
            {
                nomeFinal = txtNomeExercicioCustom.Text ?? string.Empty;
            }
            else
            {
                nomeFinal = selecionado;
            }
        }

        if (string.IsNullOrWhiteSpace(nomeFinal))
        {
            await DisplayAlert("Atenção", "Selecione ou informe o nome do exercício.", "OK");
            return;
        }

        int fichaSelecionadaIndex = pickerDiaFicha.SelectedIndex < 0 ? 0 : pickerDiaFicha.SelectedIndex;

        _todosExercicios.Add(new ItemExercicio
        {
            Id = _nextId++,
            FichaIndex = fichaSelecionadaIndex,
            GrupoMuscularKey = pickerGrupoMuscular.SelectedItem?.ToString() ?? "Geral",
            NomeCustomizado = nomeFinal,
            Series = string.IsNullOrWhiteSpace(txtSeries.Text) ? "3" : txtSeries.Text,
            Reps = string.IsNullOrWhiteSpace(txtReps.Text) ? "10" : txtReps.Text,
            Carga = string.IsNullOrWhiteSpace(txtCarga.Text) ? "0" : txtCarga.Text,
            Concluido = false
        });

        SalvarExerciciosNoStorage();
        AtualizarListaVisual();

        txtNomeExercicioCustom.Text = string.Empty;
        txtSeries.Text = string.Empty;
        txtReps.Text = string.Empty;
        txtCarga.Text = string.Empty;

        await DisplayAlert("Sucesso", "Exercício adicionado!", "OK");
    }

    private void AtualizarTextosIdioma()
    {
        Title = LocalizationService.Get("TabTreino");

        if (lblTituloFicha == null) return;

        lblTituloFicha.Text = LocalizationService.Get("MinhaFicha");
        lblDiaFicha.Text = LocalizationService.Get("SelecaoFicha");
        lblAdicionarExercicio.Text = LocalizationService.Get("AdicionarExercicio");
        lblGrupoMuscular.Text = LocalizationService.Get("GrupoMuscular");
        lblNomeExercicio.Text = LocalizationService.Get("NomeExercicio");
        lblSeries.Text = LocalizationService.Get("Series");
        lblReps.Text = LocalizationService.Get("Reps");
        lblCarga.Text = LocalizationService.Get("Carga");
        btnSalvarExercicio.Text = LocalizationService.Get("SalvarExercicio");
        lblExerciciosDaFicha.Text = LocalizationService.Get("ExerciciosSalvos");

        AtualizarPickersPorIdioma();
        AtualizarListaVisual();
    }

    private void AtualizarPickersPorIdioma()
    {
        int idxDia = pickerDiaFicha.SelectedIndex < 0 ? 0 : pickerDiaFicha.SelectedIndex;
        int idxGrupo = pickerGrupoMuscular.SelectedIndex < 0 ? 0 : pickerGrupoMuscular.SelectedIndex;

        switch (LocalizationService.IdiomaAtual)
        {
            case "en":
                pickerDiaFicha.ItemsSource = new List<string>
                {
                    "Workout A (Monday - Chest & Triceps)",
                    "Workout B (Tuesday - Back & Biceps)",
                    "Workout C (Wednesday - Lower Body)",
                    "Workout D (Thursday - Shoulders & Abs)",
                    "Workout E (Friday - Cardio / Full Body)"
                };
                pickerGrupoMuscular.ItemsSource = new List<string>
                {
                    "Chest", "Back", "Legs", "Hamstrings", "Shoulders", "Biceps", "Triceps", "Calves", "Abs"
                };
                break;
            case "es":
                pickerDiaFicha.ItemsSource = new List<string>
                {
                    "Entrenamiento A (Lunes - Pecho y Tríceps)",
                    "Entrenamiento B (Martes - Espalda y Bíceps)",
                    "Entrenamiento C (Miércoles - Piernas)",
                    "Entrenamiento D (Jueves - Hombros y Abdomen)",
                    "Entrenamiento E (Viernes - Cardio / Cuerpo Completo)"
                };
                pickerGrupoMuscular.ItemsSource = new List<string>
                {
                    "Pecho", "Espalda", "Piernas", "Isquiotibiales", "Hombros", "Bícep", "Trícep", "Pantorrillas", "Abdomen"
                };
                break;
            default:
                pickerDiaFicha.ItemsSource = new List<string>
                {
                    "Treino A (Segunda - Peito e Tríceps)",
                    "Treino B (Terça - Costas e Bíceps)",
                    "Treino C (Quarta - Membros Inferiores)",
                    "Treino D (Quinta - Ombros e Abdômen)",
                    "Treino E (Sexta - Cardio / Full Body)"
                };
                pickerGrupoMuscular.ItemsSource = new List<string>
                {
                    "Peitoral", "Dorsal / Costas", "Quadríceps / Pernas", "Posterior de Coxa", "Ombros / Deltoides", "Bíceps", "Tríceps", "Panturrilha", "Abdômen"
                };
                break;
        }

        pickerDiaFicha.SelectedIndex = idxDia;
        pickerGrupoMuscular.SelectedIndex = idxGrupo;
    }
}