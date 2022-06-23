
namespace IncomeDataStorage.Domain
{
    /// <summary>
    /// Входящие данные, которые однозначно привязаны к KeyData. Их, собственно, и нужно запомнить.
    /// (показания счетчиков)
    /// </summary>
    public class AttachedData
    {
        private int hotwater;       // ГВС
        private int coldwater;      // ХВС
        private int electricity;    // Электричество 
    }
}
