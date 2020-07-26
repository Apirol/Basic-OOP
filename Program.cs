using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace std
{

    enum MENU { LIST_STUDENT = 1, LIST_TEACHER, LIST_PERSON, SEARCH }
    interface IPerson

    {

        string Name { get; }
        string Patronomic { get; }
        string Lastname { get; }
        DateTime Date { get; }
        int Age { get; }

    }

    class Student : IPerson
    {
        public string Name { get; }
        public string Patronomic { get; }
        public string Lastname { get; }
        public DateTime Date { get; }
        public int Course { get; }
        public string Group { get; }
        public float Score { get; }
        public int Age
        {
            get
            {
                DateTime dn = DateTime.Now;
                if (dn.Month > Date.Month || dn.Month == Date.Month)
                    return dn.Year < Date.Year ?
                        dn.Year - Date.Year - 1 : dn.Year - Date.Year;
                else
                    return dn.Year - Date.Year - 1;
            }
        }
        public static Student Parse(string Data) // Добавление нового студента из файла
        {

            string[] data = Data.Split(' ');
            return new Student(data[0], data[1], data[2], int.Parse(data[3]), data[4], float.Parse(data[5]), DateTime.Parse(data[6]));
        }
        public Student(string lastname, string name, string patronomic, int course, string group, float score, DateTime date) // Конструктор
        {
            Patronomic = patronomic;
            Name = name;
            Lastname = lastname;
            Date = date;
            Course = course;
            Group = group;
            Score = score;
        }
        public Student(string s)
        {
            string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Patronomic = parts[0];
            Name = parts[1];
            Lastname = parts[2];
            Date = Convert.ToDateTime(parts[3]);
            Course = Convert.ToInt32(parts[4], 10);
            Group = parts[5];
            Score = (float)Convert.ToDouble(parts[6]);
        }
        public override string ToString()
        {
            return $"{Lastname, -15} {Name, -15} {Patronomic, -20}" +
            $"{Date, -20:d MMMM yyyy} {Age, -4}" +
            $"{Course, 10} {Group, 18} {Score, 27}";
        }
    }
    class Teacher : IPerson
    {
        public string Name { get; }
        public string Patronomic { get; }
        public string Lastname { get; }
        public DateTime Date { get; }
        public string Department { get; }
        public float WorkExperience { get; }
        public int Age
        {
            get
            {
                DateTime dn = DateTime.Now;
                if (dn.Month > Date.Month || dn.Month == Date.Month)
                    return dn.Year < Date.Year ?
                        dn.Year - Date.Year - 1 : dn.Year - Date.Year;
                else
                    return dn.Year - Date.Year - 1;
            }
        }
        public enum Position { Ассистент, СтаршийПреподаватель, Доцент, Профессор, ЗавКафедрой };
        public Position PersPos { get; }

        public static Teacher Parse(string Data) // Статическая ф-ия создания из строки
        {

            string[] data = Data.Split(' ');
            Enum.TryParse<Position>(data[5], out Position PersPos);
            return new Teacher(data[0], data[1], data[2], data[3], float.Parse(data[4]), PersPos, DateTime.Parse(data[6]));
        }
        public Teacher(string lastname, string name, string patronomic, string department, float work_experience, Position pos, DateTime date) // Конструктор
        {
            Patronomic = patronomic;
            Name = name;
            Lastname = lastname;
            Date = date;
            Department = department;
            WorkExperience = work_experience;
            PersPos = pos;
        }
        public Teacher(string s) // 
        {
            string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Patronomic = parts[0];
            Name = parts[1];
            Lastname = parts[2];
            Date = Convert.ToDateTime(parts[3]);
            Department = parts[4];
            WorkExperience = (float)Convert.ToDouble(parts[5]);
            Enum.TryParse<Position>(parts[6], out Position pos);
            PersPos = pos;
        }
        public override string ToString() // Перезапись(форматирование)
        {
            return $"{Lastname, -15} {Name, -15} {Patronomic, -20}" +
            $"{Date, -20:d MMMM yyyy} {Age, -8}" +
            $"{Department, -20} {WorkExperience, -21} {PersPos,-30}";
        }

    }
    class University
    {
        List<IPerson> persons = new List<IPerson>();
        public IEnumerable<Teacher> FindTeacher(string Departament) => persons.OfType<Teacher>().Where(p => p.Department == Departament); // Поиск преподавателя по названию кафедры
        public void AddPerson(IPerson person) => persons.Add(person); // Добавление person в University
        public IEnumerable<Student> DateSort(DateTime birthday) => persons.OfType<Student>().Where(p => p.Date >= birthday).OrderByDescending(p => p.Date); // Сортировка студентов по дате рождения
        public IEnumerable<IPerson> Persons => persons.OrderBy(p => p.Date); // Сортировка всех людей по дате рождения
        public IEnumerable<Student> Student => persons.OfType<Student>().OrderBy(p => p.Score); // Сортировка студентов по среднему баллу
        public IEnumerable<Teacher> Teachers => persons.OfType<Teacher>().OrderBy(p => p.PersPos); // Сортировка преподавателей по должности
    }
    class Program
    {
        static void Main()
        {
            University Nstu = new University();
            string[] Teachers = File.ReadAllLines("C:/Users/Редуты/source/repos/Второе ООП/Teachers.txt");
            string[] Students = File.ReadAllLines("C:/Users/Редуты/source/repos/Второе ООП/Students.txt");
            foreach (string word in Teachers)
            {
                Nstu.AddPerson(Teacher.Parse(word));
            }
            foreach (string word in Students)
            {
                Nstu.AddPerson(Student.Parse(word));
            }

            Console.WriteLine("1 - Вывести список студентов\n2 - Вывести список преподавателей\n3 - Вывести список людей\n4 - Поиск\n5 - Выход\n");
            int choice = Convert.ToInt32(Console.ReadLine());
            List<string> outputlist = new List<string> { };
            string[] headlinesForTeacher = { "Фамилия\t\tИмя\t\tОтчество\t    Дата рождения     Возраст    Кафедра    \t Стаж работы      \t    Должность" };
            string[] headlinesForStudent = { "Фамилия\t\tИмя\t\tОтчество\t    Дата рождения        Возраст    Курс              Группа    \t\tСредний балл" };
            string[] headlinesForPerson = { "Фамилия\t\tИмя\t\tОтчество\t    Дата рождения      Возраст   Кафедра/Курс\tСтаж работы/Группа          Должность/Средний балл" };

            switch (choice)
            {
                case 1:
                    using (StreamWriter outFile = new StreamWriter("output.txt", false, Encoding.Default)) // Запись символов в поток в определённой кодировке
                    {
                        outFile.WriteLine("{0,-45}", headlinesForStudent);
                        foreach (var p in Nstu.Student)
                        {
                            string data = p.ToString();
                            outFile.WriteLine(String.Format("{0,-30}", data));
                        }
                    }
                    break;
                case 2:
                    using (StreamWriter outFile = new StreamWriter("output.txt", false, Encoding.Default)) // Запись символов в поток в определённой кодировке
                    {
                        outFile.WriteLine("{0,-45}", headlinesForTeacher);
                        foreach (var p in Nstu.Teachers) 
                        {
                            string data = p.ToString();
                            outFile.WriteLine(String.Format("{0,-30}", data));
                        }
                    }
                    break;
                case 3:
                    using (StreamWriter outFile = new StreamWriter("output.txt", false, Encoding.Default)) // Запись символов в поток в определённой кодировке
                    {
                        outFile.WriteLine("{0,-45}", headlinesForPerson);
                        foreach (var p in Nstu.Persons)
                        {
                            string data = p.ToString();
                            outFile.WriteLine(String.Format("{0,-30}", data));
                        }
                    }
                    break;
                case 4:
                    Console.WriteLine("Введите название кафедры в родительном падеже\n");
                    string s = Console.ReadLine();
                    using (StreamWriter outFile = new StreamWriter("output.txt", false, Encoding.Default)) // Запись символов в поток в определённой кодировке
                    {
                        outFile.WriteLine("{0,-45}", headlinesForTeacher);
                        foreach (var p in Nstu.FindTeacher(s))
                        {
                            string data = p.ToString();
                            outFile.WriteLine(String.Format("{0,-30}", data));
                        }
                    }
                    break;
                default: break;
            }
        }
    }
}