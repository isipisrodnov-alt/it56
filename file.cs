using System;

class Rectangle
{
    // Поля для хранения длины и ширины прямоугольника
    private double length;
    private double width;

    // Конструктор по умолчанию
    public Rectangle()
    {
        length = 1.0;
        width = 1.0;
    }

    // Конструктор с параметрами
    public Rectangle(double length, double width)
    {
        this.length = length;
        this.width = width;
    }

    // Конструктор копирования
    public Rectangle(Rectangle rectangle)
    {
        this.length = rectangle.length;
        this.width = rectangle.width;
    }

    // Деструктор
    ~Rectangle()
    {
        // Здесь можно освободить ресурсы, если это необходимо
        Console.WriteLine("Деструктор вызван: прямоугольник удален.");
    }

    // Метод для вычисления площади
    public double Area()
    {
        return length * width;
    }

    // Свойства для доступа к длине и ширине
    public double Length
    {
        get { return length; }
        set { length = value; }
    }

    public double Width
    {
        get { return width; }
        set { width = value; }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Создание экземпляра класса Rectangle
        Rectangle rect1 = new Rectangle(5.0, 3.0);
        Console.WriteLine("Площадь прямоугольника: " + rect1.Area());

        // Создание экземпляра с использованием конструктора копирования
        Rectangle rect2 = new Rectangle(rect1);
        Console.WriteLine("Площадь копии прямоугольника: " + rect2.Area());
    }
}