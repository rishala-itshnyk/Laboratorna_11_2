using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Laboratorna_11_2
{
    public enum Specialization
    {
        ComputerScience,
        Informatics,
        MathematicsEconomics,
        PhysicsInformatics,
        LaborTraining
    }

    public struct Marks
    {
        public int Physics;
        public int Mathematics;
        public int Informatics;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct AdditionalMarks
    {
        [FieldOffset(0)] public int Programming;

        [FieldOffset(4)] public int NumericalMethods;

        [FieldOffset(8)] public int Pedagogy;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct StudentLevel
    {
        public int StudentNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string LastName;

        public int Course;
        public Specialization Specialization;
        public Marks SubjectMarks;
        public AdditionalMarks AdditionalMarks;
    }

    public class Program
    {
        static void Main()
        {
            Console.Write("Введіть к-сть студентів: ");
            int numberOfStudents = int.Parse(Console.ReadLine());

            Console.Write("Введіть ім'я файлу для збереження даних: ");
            string fileName = Console.ReadLine();

            SaveToFile(numberOfStudents, fileName);

            Console.WriteLine("\nДані зчитані з файлу:");
            StudentLevel[] loadedData = LoadFromFile(fileName);
            Print(loadedData);
            ComputeAverageMarks(loadedData);
            CountStudentsWithHighPhysicsMarks(loadedData);
        }

        public static void SaveToFile(int numberOfStudents, string fileName)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
                {
                    for (int i = 0; i < numberOfStudents; i++)
                    {
                        Console.WriteLine($"\nВкажіть дані студента {i + 1}:");
                        StudentLevel student;
                        
                        student.AdditionalMarks.Programming = 0;
                        student.AdditionalMarks.NumericalMethods = 0;
                        student.AdditionalMarks.Pedagogy = 0;


                        student.StudentNumber = i + 1;

                        Console.Write("Прізвище Студента: ");
                        student.LastName = Console.ReadLine();

                        Console.Write("Курс: ");
                        student.Course = int.Parse(Console.ReadLine());

                        Console.Write(
                            "Спеціалізація (0-Комп'ютерні науки, 1-Інформатика, 2-Математика та Економіка, 3-Фізика та Інформатика, 4-Трудове навчання): ");
                        student.Specialization = (Specialization)Enum.Parse(typeof(Specialization), Console.ReadLine());

                        Console.Write("Оцінка з Фізика: ");
                        student.SubjectMarks.Physics = int.Parse(Console.ReadLine());

                        Console.Write("Оцінка з Математики: ");
                        student.SubjectMarks.Mathematics = int.Parse(Console.ReadLine());

                        switch (student.Specialization)
                        {
                            case Specialization.ComputerScience:
                                Console.Write("Оцінка з Програмування: ");
                                student.AdditionalMarks.Programming = int.Parse(Console.ReadLine());
                                break;
                            case Specialization.Informatics:
                                Console.Write("Оцінка з Чисельних Методів: ");
                                student.AdditionalMarks.NumericalMethods = int.Parse(Console.ReadLine());
                                break;
                            default:
                                Console.Write("Оцінка з Педагогіки: ");
                                student.AdditionalMarks.Pedagogy = int.Parse(Console.ReadLine());
                                break;
                        }

                        writer.Write(student.StudentNumber);
                        writer.Write(student.LastName.PadRight(50, '\0').ToCharArray());
                        writer.Write(student.Course);
                        writer.Write((int)student.Specialization);
                        writer.Write(student.SubjectMarks.Physics);
                        writer.Write(student.SubjectMarks.Mathematics);

                        switch (student.Specialization)
                        {
                            case Specialization.ComputerScience:
                                writer.Write(student.AdditionalMarks.Programming);
                                break;
                            case Specialization.Informatics:
                                writer.Write(student.AdditionalMarks.NumericalMethods);
                                break;
                            default:
                                writer.Write(student.AdditionalMarks.Pedagogy);
                                break;
                        }
                    }
                }
                Console.WriteLine($"Дані збережено у файл {fileName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні файлу: {ex.Message}");
            }
        }

        public static StudentLevel[] LoadFromFile(string fileName)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    int sizeOfStudent = Marshal.SizeOf<StudentLevel>();
                    int numberOfStudents = (int)fileInfo.Length / sizeOfStudent;

                    StudentLevel[] loadedData = new StudentLevel[numberOfStudents];

                    for (int i = 0; i < numberOfStudents; i++)
                    {
                        loadedData[i].StudentNumber = reader.ReadInt32();
                        loadedData[i].LastName = new string(reader.ReadChars(50));
                        loadedData[i].Course = reader.ReadInt32();
                        loadedData[i].Specialization = (Specialization)reader.ReadInt32();
                        loadedData[i].SubjectMarks.Physics = reader.ReadInt32();
                        loadedData[i].SubjectMarks.Mathematics = reader.ReadInt32();

                        switch (loadedData[i].Specialization)
                        {
                            case Specialization.ComputerScience:
                                loadedData[i].AdditionalMarks.Programming = reader.ReadInt32();
                                break;
                            case Specialization.Informatics:
                                loadedData[i].AdditionalMarks.NumericalMethods = reader.ReadInt32();
                                break;
                            default:
                                loadedData[i].AdditionalMarks.Pedagogy = reader.ReadInt32();
                                break;
                        }
                    }

                    Console.WriteLine($"Дані зчитано з файлу {fileName}.");
                    return loadedData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні файлу: {ex.Message}");
                return Array.Empty<StudentLevel>();
            }
        }

        public static void Print(StudentLevel[] students)
        {
            Console.WriteLine(
                "\n| № | Прізвище | Курс | Спеціалізація | Фізика | Математика | Програмування | Чисельні Методи | Педагогіка |");
            Console.WriteLine(
                "--------------------------------------------------------------------------------------------------------");
            for (int i = 0; i < students.Length; i++)
            {
                Console.WriteLine(
                    $"| {students[i].StudentNumber,2} | {students[i].LastName,-10} | {students[i].Course,4} | {students[i].Specialization,-13} | {students[i].SubjectMarks.Physics,6} | {students[i].SubjectMarks.Mathematics,10} | {students[i].AdditionalMarks.Programming,13} | {students[i].AdditionalMarks.NumericalMethods,14} | {students[i].AdditionalMarks.Pedagogy,10} |");
            }
        }

        public static void ComputeAverageMarks(StudentLevel[] students)
        {
            double averagePhysics = students.Any() ? students.Average(s => s.SubjectMarks.Physics) : 0;
            double averageMathematics = students.Any() ? students.Average(s => s.SubjectMarks.Mathematics) : 0;
            double averageProgramming = students.Where(s => s.Specialization == Specialization.ComputerScience).Any()
                ? students.Where(s => s.Specialization == Specialization.ComputerScience)
                    .Average(s => s.AdditionalMarks.Programming)
                : 0;
            double averageNumericalMethods = students.Where(s => s.Specialization == Specialization.Informatics).Any()
                ? students.Where(s => s.Specialization == Specialization.Informatics)
                    .Average(s => s.AdditionalMarks.NumericalMethods)
                : 0;
            double averagePedagogy = students.Where(s => s.Specialization == Specialization.LaborTraining).Any()
                ? students.Where(s => s.Specialization == Specialization.LaborTraining)
                    .Average(s => s.AdditionalMarks.Pedagogy)
                : 0;

            Console.WriteLine(
                $"\nСередні оцінки з різних предметів:\nФізика: {averagePhysics}\nМатематика: {averageMathematics}\nСередня Оцінка з Програмування: {averageProgramming}\nСередня Оцінка з Чисельних Методів: {averageNumericalMethods}\nСередня Оцінка з Педагогіки: {averagePedagogy}");
        }

        public static void CountStudentsWithHighPhysicsMarks(StudentLevel[] students)
        {
            int highPhysicsCount = students.Count(s => s.SubjectMarks.Physics == 5 || s.SubjectMarks.Physics == 4);

            Console.WriteLine($"\nК-сть студентів з високою оцінкою з Фізики (5 або 4): {highPhysicsCount}");
        }
    }
}
