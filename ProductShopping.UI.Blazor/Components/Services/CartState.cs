namespace ProductShopping.UI.Blazor.Services;

/// <summary>
/// Wspólny stan koszyka dla jednego obwodu (circuit) Blazor Server.
/// Products i Cart aktualizują liczbę produktów, a NavMenu nasłuchuje zmian
/// i odświeża licznik w górnym pasku.
/// Rejestracja w Program.cs: builder.Services.AddScoped&lt;CartState&gt;();
/// </summary>
public sealed class CartState
{
    public int ItemsCount { get; private set; }

    public event Action? Changed;

    /// <summary>Ustawia dokładną liczbę produktów w koszyku (np. po pobraniu koszyka z API).</summary>
    public void SetItemsCount(int count)
    {
        count = Math.Max(0, count);

        if (count == ItemsCount)
            return;

        ItemsCount = count;
        Changed?.Invoke();
    }

    /// <summary>Zmienia liczbę produktów o podaną wartość (np. +3 po dodaniu, -1 po zmniejszeniu).</summary>
    public void ChangeItemsCount(int delta) => SetItemsCount(ItemsCount + delta);

    public void Clear() => SetItemsCount(0);
}
