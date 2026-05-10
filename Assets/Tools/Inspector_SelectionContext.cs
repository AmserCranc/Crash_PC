public static class NSFSelection
{
    public static object ActiveObject;

    public static System.Action<object> SelectionChanged;

    public static void Set(object obj)
    {
        ActiveObject = obj;
        SelectionChanged?.Invoke(obj);
    }
}