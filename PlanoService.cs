namespace MyBody;

public enum Objetivo
{
    PerdaDeGordura,   // Cutting
    GanhoDeMassa,     // Bulking
    Recomposicao      // Baixar BF mantendo massa
}

public class PerfilUsuario
{
    public double PesoKg { get; set; }
    public double BfAtual { get; set; }
    public double BfMeta { get; set; }
    public Objetivo ObjetivoPrincipal { get; set; }
}

public class PlanoResultado
{
    public double CaloriasDiarias { get; set; }
    public double ProteinaGrams { get; set; }
    public double CarboidratoGrams { get; set; }
    public double GorduraGrams { get; set; }
    public string SugestaoTreino { get; set; } = string.Empty;
}

public class PlanoService
{
    public PlanoResultado CalcularPlano(PerfilUsuario perfil)
    {
        // 1. Estimativa de Massa Magra (Fórmula simplificada de Katch-McArdle)
        double massaMagra = perfil.PesoKg * (1 - (perfil.BfAtual / 100.0));

        // Taxa Metabólica Basal aproximada baseada na massa magra
        double tmb = 370 + (21.6 * massaMagra);

        // Considera o gasto do treino pesado (Fator ~1.55)
        double gastoGastoTotal = tmb * 1.55;

        double caloriasMeta = gastoGastoTotal;
        string sugestaoTreino = "";

        // 2. Ajuste conforme o Objetivo (Ex: Descer de 15% para 10% BF)
        if (perfil.ObjetivoPrincipal == Objetivo.PerdaDeGordura || perfil.BfAtual > perfil.BfMeta)
        {
            // Déficit calórico moderado para preservar massa magra
            caloriasMeta -= 400;
            sugestaoTreino = "Treino de Musculação com Carga Alta (Preservação) + 20-30min de Cardio Pós-Treino.";
        }
        else if (perfil.ObjetivoPrincipal == Objetivo.GanhoDeMassa)
        {
            caloriasMeta += 300; // Superávit controlado
            sugestaoTreino = "Treino com Foco em Hipertrofia e Progressão de Carga (ABCD/ABCDE).";
        }
        else
        {
            sugestaoTreino = "Treino Intenso de Musculação + Cardio Moderado alternado.";
        }

        // 3. Distribuição de Macronutrientes (Ajustado para quem treina forte)
        // Proteína alta: ~2.2g por kg de massa magra ou peso total
        double proteina = perfil.PesoKg * 2.2;
        double gordura = perfil.PesoKg * 0.8;  // Gorduras essenciais (~0.8g/kg)

        // Restante das calorias em Carboidratos (4 kcal/g)
        double calProteina = proteina * 4;
        double calGordura = gordura * 9;
        double calCarbo = caloriasMeta - (calProteina + calGordura);
        double carboidrato = calCarbo / 4;

        return new PlanoResultado
        {
            CaloriasDiarias = Math.Round(caloriasMeta),
            ProteinaGrams = Math.Round(proteina),
            CarboidratoGrams = Math.Round(carboidrato),
            GorduraGrams = Math.Round(gordura),
            SugestaoTreino = sugestaoTreino
        };
    }
}