    using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace IncomeDataStorage
{
    // Очень простой пример, демонстрирующий событие. 

    // Объявить тип делегата для события, 
    delegate void MyEventHandler() ; 

    // Объявить класс, содержащий событие, 
    class MyEvent 
    { 
        public event MyEventHandler SomeEvent; 

        // Этот метод вызывается для запуска события, 
        public void OnSomeEvent() 
        { 
            if(SomeEvent != null)   
            SomeEvent(); 
        } 
    }
 
    class EventDemo 
    { 
        // Обработчик события, 
        static void Handler()   
        { 
            Console.WriteLine("Произошло событие"); 
        } 

        static void Main() 
        { 
            MyEvent evt = new MyEvent(); 
            // Добавить метод Handler() в список событий, 
            evt.SomeEvent += Handler; 
            // Запустить событие, 
            evt.OnSomeEvent (); 
        } 
    } 

}
