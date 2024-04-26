using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
[Serializable]
public class Patient
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class HospitalQueue
{
    private LinkedList<Patient> patients = new LinkedList<Patient>();

    // Делегат для сортування та фільтрації
    public delegate bool PatientFilter(Patient patient);

    public void AddPatient(Patient patient)
    {
        patients.AddLast(patient);
        Console.WriteLine($"Пацієнт {patient.Name} додан у чергу. ID пацієнта: {patient.Id}");
    }

    public void RemovePatient(Guid id)
    {
        var patientToRemove = patients.FirstOrDefault(p => p.Id == id);

        if (patientToRemove == null)
        {
            Console.WriteLine($"Пацієнт з ID {id} не доданий у чергу.");
            return;
        }

        patients.Remove(patientToRemove);
        Console.WriteLine($"Пацієнт {patientToRemove.Name} видалений з черги.");
    }

    public void DisplayQueue()
    {
        Console.WriteLine("Черга пацієнтів:");
        foreach (var patient in patients)
        {
            Console.WriteLine($"ID: {patient.Id}, Ім'я: {patient.Name}, Вік: {patient.Age}");
        }
    }

    public void SaveData(string filename)
    {
        using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, patients);
            Console.WriteLine("Данні збережено.");
        }
    }

    public void LoadData(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine($"Файл {filename} не знайден.");
            return;
        }

        using (FileStream fs = new FileStream(filename, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            patients = (LinkedList<Patient>)formatter.Deserialize(fs);
            Console.WriteLine("Данні завантажені.");
        }
    }

    // Функция для сортировки пациентов по имени и возрасту
    public void SortPatients()
    {
        var sortedPatients = patients.OrderBy(p => p.Age).ThenBy(p => p.Name).ToList();
        patients = new LinkedList<Patient>(sortedPatients);
    }

    // Функция для фильтрации пациентов по имени и возрасту
    public LinkedList<Patient> FilterPatients(PatientFilter filter)
    {
        var filteredPatients = patients.Where(p => filter(p)).ToList();
        return new LinkedList<Patient>(filteredPatients);
    }
}

class Program
{
    static void Main(string[] args)
    {
        HospitalQueue hospitalQueue = new HospitalQueue();

        while (true)
        {
            Console.WriteLine("1. Дадати пацієнта");
            Console.WriteLine("2. Видалити пацієнта");
            Console.WriteLine("3. Показати чергу");
            Console.WriteLine("4. Зберегти данні");
            Console.WriteLine("5. Завантажити данні");
            Console.WriteLine("6. Відсортувати пацієнтів");
            Console.WriteLine("7. Фільтрація пацієнтів");
            Console.WriteLine("8. Вихід");
            Console.Write("Обрати дію: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Ввести ім'я пацієнта: ");
                    string name = Console.ReadLine();
                    Console.Write("Ввести вік пацієнта: ");
                    int age = Convert.ToInt32(Console.ReadLine());
                    hospitalQueue.AddPatient(new Patient { Id = Guid.NewGuid(), Name = name, Age = age });
                    break;
                case "2":
                    Console.Write("Ввести ID пацієнта для видалення: ");
                    Guid idToRemove;
                    if (Guid.TryParse(Console.ReadLine(), out idToRemove))
                    {
                        hospitalQueue.RemovePatient(idToRemove);
                    }
                    else
                    {
                        Console.WriteLine("Невірний формат ID. Будь ласка, спробуйте знову.");
                    }
                    break;
                case "3":
                    hospitalQueue.DisplayQueue();
                    break;
                case "4":
                    Console.Write("Введіть ім'я файлу для збереження данних: ");
                    string saveFilename = Console.ReadLine();
                    hospitalQueue.SaveData(saveFilename);
                    break;
                case "5":
                    Console.Write("Введіть ім'я файлу для завантаження данних: ");
                    string loadFilename = Console.ReadLine();
                    hospitalQueue.LoadData(loadFilename);
                    break;
                case "6":
                    hospitalQueue.SortPatients();
                    break;
                case "7":
                    Console.WriteLine("Відфільтрувати пацієнтів старше 18 років");
                    HospitalQueue.PatientFilter filter = delegate(Patient p) { return p.Age > 18; };
                    var filteredPatients = hospitalQueue.FilterPatients(filter);
                    foreach (var patient in filteredPatients)
                    {
                        Console.WriteLine($"ID: {patient.Id}, Ім'я: {patient.Name}, Вік: {patient.Age}");
                    }
                    break;
                case "8":
                    return;
                default:
                    Console.WriteLine("Невірний вибір. Будь ласка, спробуйте знову.");
                    break;
            }
        }
            
    }
}

