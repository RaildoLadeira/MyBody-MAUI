using Microsoft.Maui.Controls;
using System.Linq;

namespace MyBody;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    public void AtualizarTitulosAbas()
    {
        var shellContents = this.Items
            .SelectMany(i => i.Items)
            .SelectMany(section => section.Items)
            .ToList();

        if (shellContents.Count >= 3)
        {
            shellContents[0].Title = LocalizationService.Get("TabDieta");
            shellContents[1].Title = LocalizationService.Get("TabTreino");
            shellContents[2].Title = LocalizationService.Get("TabEvolucao");
        }
        else if (Items.Count > 0 && Items[0] is TabBar tabBar && tabBar.Items.Count >= 3)
        {
            tabBar.Items[0].Title = LocalizationService.Get("TabDieta");
            tabBar.Items[1].Title = LocalizationService.Get("TabTreino");
            tabBar.Items[2].Title = LocalizationService.Get("TabEvolucao");
        }
    }
}