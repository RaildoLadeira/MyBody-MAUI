namespace MyBody;

public enum Genero
{
    Masculino,
    Feminino
}

public enum NivelAtividade
{
    Sedentario,
    Leve,
    Moderado,
    Intenso
}

public class RefeicaoItem
{
    public TimeSpan Horario { get; set; }
    public string NomeRefeicao { get; set; } = string.Empty;
    public string DescricaoAlimentos { get; set; } = string.Empty;
    public int Calorias { get; set; }
}

public class TreinoDiario
{
    public TimeSpan HorarioTreino { get; set; }
    public string TituloTreino { get; set; } = string.Empty;
}