
namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Маска, которая наложена на ячейку первичного набора данных из таблицы
    /// экселя. Необходима для согласования с другими наборами данных из этой таблицы.
    /// </summary>  
    public class Mask
    {
        /// <summary>
        /// Содержит ли ячейка значение.
        /// </summary>
        public bool HasValue { get; set; }
        /// <summary>
        /// Является ли ячейка заголовком.
        /// </summary>
        public bool IsHeader { get; set; }
        /// <summary>
        /// Указывает, является ли маска "сложной" и в ней "зашифровано" значение.
        /// </summary>
        public bool IsComplexMask { get; set; }
        /// <summary>
        /// Синтаксическое представление маски
        /// </summary>
        public string MaskSyntax { get; set; }
        /// <summary>
        /// Жопоиндекс :) Необходим для уточнения ассоциации с другими ячейками из таблицы.
        /// </summary>
        public int AssIndex { get; set; }
        /// <summary>
        /// Количество индексов ассоциации, необходим для объединения нескольких 
        /// ключевых ячеек по значению.
        /// </summary>
        public int АssIndexCount { get; set; }
    }
}
