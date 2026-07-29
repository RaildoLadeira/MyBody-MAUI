using SQLite;

namespace MyBody;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "mybody.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<ExercicioDb>();
        await _database.CreateTableAsync<MedicaoDb>();
        await _database.CreateTableAsync<PerfilDb>();
        await _database.CreateTableAsync<HistoricoCargaDb>();
        await _database.CreateTableAsync<RefeicaoDb>();
        await _database.CreateTableAsync<ConsumoAguaDb>();
        await _database.CreateTableAsync<AlimentoDb>();

        await InicializarTreinosPadraoAsync();
        await InicializarAlimentosPadraoAsync();
    }

    private async Task InicializarTreinosPadraoAsync()
    {
        if (_database is null) return;

        var existemExercicios = await _database.Table<ExercicioDb>().CountAsync();
        if (existemExercicios == 0)
        {
            var treinosIniciais = new List<ExercicioDb>
            {
                new ExercicioDb { GrupoMuscular = "Treino A (Seg) - Peito/Tríceps/Ombro", NomeExercicio = "Supino Reto com Barra", Series = 4, Repeticoes = "8-10", CargaKg = 30 },
                new ExercicioDb { GrupoMuscular = "Treino A (Seg) - Peito/Tríceps/Ombro", NomeExercicio = "Supino Inclinado com Halteres", Series = 4, Repeticoes = "10-12", CargaKg = 22 },
                new ExercicioDb { GrupoMuscular = "Treino A (Seg) - Peito/Tríceps/Ombro", NomeExercicio = "Crossover na Polia", Series = 3, Repeticoes = "12-15", CargaKg = 15 },
                new ExercicioDb { GrupoMuscular = "Treino A (Seg) - Peito/Tríceps/Ombro", NomeExercicio = "Tríceps Corda", Series = 4, Repeticoes = "10-12", CargaKg = 20 },
                new ExercicioDb { GrupoMuscular = "Treino A (Seg) - Peito/Tríceps/Ombro", NomeExercicio = "Elevação Lateral Halteres", Series = 4, Repeticoes = "12-15", CargaKg = 10 },

                new ExercicioDb { GrupoMuscular = "Treino B (Ter) - Costas/Bíceps", NomeExercicio = "Puxada Alta Frontal", Series = 4, Repeticoes = "10-12", CargaKg = 45 },
                new ExercicioDb { GrupoMuscular = "Treino B (Ter) - Costas/Bíceps", NomeExercicio = "Remada Curvada com Barra", Series = 4, Repeticoes = "8-10", CargaKg = 35 },
                new ExercicioDb { GrupoMuscular = "Treino B (Ter) - Costas/Bíceps", NomeExercicio = "Remada Baixa Triângulo", Series = 3, Repeticoes = "10-12", CargaKg = 40 },
                new ExercicioDb { GrupoMuscular = "Treino B (Ter) - Costas/Bíceps", NomeExercicio = "Rosca Direta no W", Series = 4, Repeticoes = "10-12", CargaKg = 14 },
                new ExercicioDb { GrupoMuscular = "Treino B (Ter) - Costas/Bíceps", NomeExercicio = "Rosca Martelo", Series = 3, Repeticoes = "10-12", CargaKg = 12 },

                new ExercicioDb { GrupoMuscular = "Treino C (Qui) - Pernas Completo", NomeExercicio = "Agachamento Livre", Series = 4, Repeticoes = "8-10", CargaKg = 40 },
                new ExercicioDb { GrupoMuscular = "Treino C (Qui) - Pernas Completo", NomeExercicio = "Leg Press 45°", Series = 4, Repeticoes = "10-12", CargaKg = 120 },
                new ExercicioDb { GrupoMuscular = "Treino C (Qui) - Pernas Completo", NomeExercicio = "Cadeira Extensora", Series = 3, Repeticoes = "12-15", CargaKg = 35 },
                new ExercicioDb { GrupoMuscular = "Treino C (Qui) - Pernas Completo", NomeExercicio = "Mesa Flexora", Series = 4, Repeticoes = "10-12", CargaKg = 30 },
                new ExercicioDb { GrupoMuscular = "Treino C (Qui) - Pernas Completo", NomeExercicio = "Gêmeos em Pé (Panturrilha)", Series = 4, Repeticoes = "15-20", CargaKg = 50 },

                new ExercicioDb { GrupoMuscular = "Treino D (Sex) - Ombros/Core", NomeExercicio = "Desenvolvimento com Halteres", Series = 4, Repeticoes = "8-10", CargaKg = 18 },
                new ExercicioDb { GrupoMuscular = "Treino D (Sex) - Ombros/Core", NomeExercicio = "Elevação Frontal", Series = 3, Repeticoes = "10-12", CargaKg = 8 },
                new ExercicioDb { GrupoMuscular = "Treino D (Sex) - Ombros/Core", NomeExercicio = "Encolhimento com Halteres", Series = 4, Repeticoes = "12-15", CargaKg = 24 },
                new ExercicioDb { GrupoMuscular = "Treino D (Sex) - Ombros/Core", NomeExercicio = "Abdominal Supra na Polia", Series = 4, Repeticoes = "15-20", CargaKg = 30 }
            };

            foreach (var ex in treinosIniciais)
            {
                await _database.InsertAsync(ex);
                await _database.InsertAsync(new HistoricoCargaDb
                {
                    ExercicioId = ex.Id,
                    DataRegistro = DateTime.Now,
                    CargaKg = ex.CargaKg,
                    Observacao = "Carga Inicial Padronizada"
                });
            }
        }
    }

    private async Task InicializarAlimentosPadraoAsync()
    {
        if (_database is null) return;

        var existemAlimentos = await _database.Table<AlimentoDb>().CountAsync();
        if (existemAlimentos == 0)
        {
            var alimentosIniciais = new List<AlimentoDb>
            {
                new AlimentoDb { Nome = "Ovo Cozido", ProteinaPor100g = 13, CarboPor100g = 1.1, GorduraPor100g = 11 },
                new AlimentoDb { Nome = "Peito de Frango Grelhado", ProteinaPor100g = 31, CarboPor100g = 0, GorduraPor100g = 3.6 },
                new AlimentoDb { Nome = "Arroz Branco Cozido", ProteinaPor100g = 2.5, CarboPor100g = 28, GorduraPor100g = 0.2 },
                new AlimentoDb { Nome = "Arroz Integral Cozido", ProteinaPor100g = 2.6, CarboPor100g = 23, GorduraPor100g = 1 },
                new AlimentoDb { Nome = "Feijão Preto Cozido", ProteinaPor100g = 4.5, CarboPor100g = 14, GorduraPor100g = 0.5 },
                new AlimentoDb { Nome = "Batata Doce Cozida", ProteinaPor100g = 1.6, CarboPor100g = 20, GorduraPor100g = 0.1 },
                new AlimentoDb { Nome = "Patinho Moído Grelhado", ProteinaPor100g = 35, CarboPor100g = 0, GorduraPor100g = 7 },
                new AlimentoDb { Nome = "Banana Prata", ProteinaPor100g = 1.3, CarboPor100g = 26, GorduraPor100g = 0.3 },
                new AlimentoDb { Nome = "Aveia em Flocos", ProteinaPor100g = 14, CarboPor100g = 67, GorduraPor100g = 7 },
                new AlimentoDb { Nome = "Whey Protein (Pó)", ProteinaPor100g = 80, CarboPor100g = 6, GorduraPor100g = 3 },
                new AlimentoDb { Nome = "Pasta de Amendoim", ProteinaPor100g = 28, CarboPor100g = 20, GorduraPor100g = 50 },
                new AlimentoDb { Nome = "Pão de Forma Integral", ProteinaPor100g = 9, CarboPor100g = 43, GorduraPor100g = 3.5 },
                new AlimentoDb { Nome = "Azeite de Oliva Extra Virgem", ProteinaPor100g = 0, CarboPor100g = 0, GorduraPor100g = 100 },
                new AlimentoDb { Nome = "Tapioca (Goma)", ProteinaPor100g = 0, CarboPor100g = 54, GorduraPor100g = 0 },
                new AlimentoDb { Nome = "Queijo Cottage / Creme de Ricota", ProteinaPor100g = 12, CarboPor100g = 3, GorduraPor100g = 4 }
            };

            foreach (var alimento in alimentosIniciais)
            {
                await _database.InsertAsync(alimento);
            }
        }
    }

    public async Task<List<AlimentoDb>> GetAlimentosAsync()
    {
        await InitAsync();
        return await _database!.Table<AlimentoDb>().OrderBy(a => a.Nome).ToListAsync();
    }

    public async Task<int> SaveAlimentoAsync(AlimentoDb alimento)
    {
        await InitAsync();
        return await _database!.InsertAsync(alimento);
    }

    public async Task<int> DeleteAlimentoAsync(AlimentoDb alimento)
    {
        await InitAsync();
        return await _database!.DeleteAsync(alimento);
    }

    public async Task<PerfilDb?> GetPerfilAsync()
    {
        await InitAsync();
        var perfis = await _database!.Table<PerfilDb>().ToListAsync();
        return perfis.FirstOrDefault();
    }

    public async Task SavePerfilAsync(PerfilDb perfil)
    {
        await InitAsync();
        var existente = await GetPerfilAsync();
        if (existente == null)
        {
            await _database!.InsertAsync(perfil);
        }
        else
        {
            perfil.Id = existente.Id;
            await _database!.UpdateAsync(perfil);
        }
    }

    public async Task<List<ExercicioDb>> GetExerciciosAsync()
    {
        await InitAsync();
        return await _database!.Table<ExercicioDb>().ToListAsync();
    }

    public async Task<int> SaveExercicioAsync(ExercicioDb exercicio)
    {
        await InitAsync();
        if (exercicio.Id != 0)
            return await _database!.UpdateAsync(exercicio);
        return await _database!.InsertAsync(exercicio);
    }

    public async Task<int> DeleteExercicioAsync(ExercicioDb exercicio)
    {
        await InitAsync();
        return await _database!.DeleteAsync(exercicio);
    }

    public async Task<List<HistoricoCargaDb>> GetHistoricoCargasAsync(int exercicioId)
    {
        await InitAsync();
        return await _database!.Table<HistoricoCargaDb>()
                                .Where(h => h.ExercicioId == exercicioId)
                                .OrderByDescending(h => h.DataRegistro)
                                .ToListAsync();
    }

    public async Task<int> SaveCargaSemanalAsync(HistoricoCargaDb carga)
    {
        await InitAsync();
        return await _database!.InsertAsync(carga);
    }

    public async Task<List<RefeicaoDb>> GetRefeicoesDoDiaAsync(DateTime data)
    {
        await InitAsync();
        var inicio = data.Date;
        var fim = data.Date.AddDays(1);
        return await _database!.Table<RefeicaoDb>()
                                .Where(r => r.DataHora >= inicio && r.DataHora < fim)
                                .ToListAsync();
    }

    public async Task<int> SaveRefeicaoAsync(RefeicaoDb refeicao)
    {
        await InitAsync();
        return await _database!.InsertAsync(refeicao);
    }

    public async Task<int> DeleteRefeicaoAsync(RefeicaoDb refeicao)
    {
        await InitAsync();
        return await _database!.DeleteAsync(refeicao);
    }

    public async Task<int> GetAguaTotalHojeAsync()
    {
        await InitAsync();
        var inicio = DateTime.Today;
        var fim = DateTime.Today.AddDays(1);
        var lista = await _database!.Table<ConsumoAguaDb>()
                                    .Where(a => a.DataHora >= inicio && a.DataHora < fim)
                                    .ToListAsync();
        return lista.Sum(a => a.MilliLitros);
    }

    public async Task<int> AddAguaAsync(int ml)
    {
        await InitAsync();
        var registro = new ConsumoAguaDb { DataHora = DateTime.Now, MilliLitros = ml };
        return await _database!.InsertAsync(registro);
    }

    public async Task DesfazerUltimaAguaAsync()
    {
        await InitAsync();
        var inicio = DateTime.Today;
        var fim = DateTime.Today.AddDays(1);
        var ultimo = await _database!.Table<ConsumoAguaDb>()
                                     .Where(a => a.DataHora >= inicio && a.DataHora < fim)
                                     .OrderByDescending(a => a.DataHora)
                                     .FirstOrDefaultAsync();
        if (ultimo != null)
        {
            await _database.DeleteAsync(ultimo);
        }
    }

    public async Task ResetAguaHojeAsync()
    {
        await InitAsync();
        var inicio = DateTime.Today;
        var fim = DateTime.Today.AddDays(1);
        var lista = await _database!.Table<ConsumoAguaDb>()
                                    .Where(a => a.DataHora >= inicio && a.DataHora < fim)
                                    .ToListAsync();
        foreach (var item in lista)
        {
            await _database.DeleteAsync(item);
        }
    }

    public async Task<List<MedicaoDb>> GetMedicoesAsync()
    {
        await InitAsync();
        return await _database!.Table<MedicaoDb>().OrderByDescending(m => m.Data).ToListAsync();
    }

    public async Task<int> SaveMedicaoAsync(MedicaoDb medicao)
    {
        await InitAsync();
        return await _database!.InsertAsync(medicao);
    }

    public async Task<int> DeleteMedicaoAsync(MedicaoDb medicao)
    {
        await InitAsync();
        return await _database!.DeleteAsync(medicao);
    }
}

public class AlimentoDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double ProteinaPor100g { get; set; }
    public double CarboPor100g { get; set; }
    public double GorduraPor100g { get; set; }
    public double CaloriasPor100g => (ProteinaPor100g * 4) + (CarboPor100g * 4) + (GorduraPor100g * 9);
}

public class PerfilDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Peso { get; set; }
    public double Altura { get; set; }
    public int Idade { get; set; }
    public double BfAtual { get; set; }
    public double BfMeta { get; set; }
    public int SexoIndex { get; set; }
    public int AtividadeIndex { get; set; }
}

public class ExercicioDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string GrupoMuscular { get; set; } = string.Empty;
    public string NomeExercicio { get; set; } = string.Empty;
    public int Series { get; set; }
    public string Repeticoes { get; set; } = string.Empty;
    public double CargaKg { get; set; }
}

public class HistoricoCargaDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ExercicioId { get; set; }
    public DateTime DataRegistro { get; set; } = DateTime.Now;
    public double CargaKg { get; set; }
    public string Observacao { get; set; } = string.Empty;
}

public class RefeicaoDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime DataHora { get; set; } = DateTime.Now;
    public string NomeRefeicao { get; set; } = string.Empty;
    public double Proteinas { get; set; }
    public double Carboidratos { get; set; }
    public double Gorduras { get; set; }
    public double Calorias => (Proteinas * 4) + (Carboidratos * 4) + (Gorduras * 9);
}

public class ConsumoAguaDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime DataHora { get; set; } = DateTime.Now;
    public int MilliLitros { get; set; }
}

public class MedicaoDb
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public double PesoKg { get; set; }
    public double BfPorcentagem { get; set; }
}