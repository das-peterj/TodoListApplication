using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoListApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TodoListEntities context = new TodoListEntities())
            {
                // deletes all previous data in the table "Assets"
                // TODO: Remove code below when product is done. Otherwise new user inputted tasks will not be saved.
                //context.Database.ExecuteSqlCommand("TRUNCATE TABLE [ToDo]");
                Console.ForegroundColor = ConsoleColor.White;
                do
                {
                    int tasksNotDone = context.Todoes.Count(x => x.Status == 0);
                    int tasksDone = context.Todoes.Count(x => x.Status == 1);
                    int userInput = 0;

                    Console.WriteLine($">> Welcome to ToDoLy\n>> You have {tasksNotDone} tasks to do and {tasksDone} tasks are done!\n>> Pick an option:");
                    Console.WriteLine(">> (1) Show Task List (by date or project)");
                    Console.WriteLine(">> (2) Add New Task");
                    Console.WriteLine(">> (3) Edit Task(update, mark as done, remove)");
                    Console.WriteLine(">> (4) Save and Quit");
                    Console.WriteLine(">> (5) Load Sample Data");
                    Console.WriteLine(">> (6) Delete all data inside the database");
                    do
                    {
                        try
                        {
                            userInput = Int32.Parse(Console.ReadLine());
                        }
                        catch (FormatException e) { Console.WriteLine($"Program only accepts inputs 1-4 | {e.Message}"); }
                        if (userInput <= 6) { break; }
                    } while (true);

                    // Gathers all the database data and puts it into a list.
                    List<Todo> toDoList = GatherDBData();

                    if (userInput == 1) // >> (1) Show Task List (by date or project)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Press (1) to Show Task List sorted by dates");
                        Console.WriteLine("Press (2) to Show Task List sorted by projects");
                        do
                        {
                            try
                            {
                                userInput = Int32.Parse(Console.ReadLine());
                            }
                            catch (FormatException e) { Console.WriteLine($"Program only accepts inputs 1-4 | {e.Message}"); }
                            if (userInput == 1 || userInput == 2) { break; }
                        } while (true);

                        WriteCategoriesInConsole();

                        if (userInput == 1) // Press (1) to Show Task List sorted by dates
                        {
                            List<Todo> sortedByDate = toDoList.OrderBy(x => x.Due_Date).ToList();
                            foreach (Todo task in sortedByDate)
                            {
                                OutputToDoList(task);
                            }
                        }
                        else if (userInput == 2) // Press (2) to Show Task List sorted by projects
                        {
                            List<Todo> sortedByProject = toDoList.OrderBy(x => x.Project).ToList();
                            foreach (Todo task in sortedByProject)
                            {
                                OutputToDoList(task);
                            }
                        }

                        EndOfToDoList();


                    }
                    else if (userInput == 2) // (2) Add New Task
                    {
                        AddNewTask();
                    }
                    else if (userInput == 3) // (3) Edit Task(update, mark as done, remove)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine(
                            "Would you like to update a task, press (1)\n" +
                            "Would you like to mark a task as done, press (2)\n" +
                            "Would you like to remove a task, press (3)"
                            );

                        // Checks if user's input is valid
                        int userInputEditTask;
                        do
                        {
                            userInputEditTask = Int32.Parse(Console.ReadLine());
                            if (userInputEditTask == 1 || userInputEditTask == 2 || userInputEditTask == 3) { break; }
                            Console.WriteLine("Try again.");
                        } while (true);

                        Console.WriteLine("");
                        Console.WriteLine("");

                        // Update a task
                        if (userInputEditTask == 1) { UpdateTask(); }

                        // For marking a task as done
                        if (userInputEditTask == 2) { MarkTaskAsDone(); }

                        // For removing a task
                        if (userInputEditTask == 3) { DeleteTask(); }

                    }
                    else if (userInput == 4) // (4) Save and Quit
                    {
                        break;
                    }
                    else if (userInput == 5) // (5) Load Sample Data
                    {
                        LoadSampleData();
                        Console.Clear();
                        Console.WriteLine("Sample Data has been loaded.");
                    }
                    else if (userInput == 6) // (6) Delete all entries inside the database
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Are you sure you wish to delete all data inside the database? This operation cannot be reverted.");
                        Console.WriteLine("Type \"Yes\" or click Enter to skip this action.");

                        string userChoice = Console.ReadLine();
                        if (userChoice.ToLower().Equals("yes"))
                        {
                            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [ToDo]");
                            Console.Clear();
                            Console.WriteLine("All data inside database has been deleted.");
                        }
                        else
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("All data will NOT be deleted.");
                            Console.ForegroundColor = ConsoleColor.White;

                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } while (true);



                // ------------------------------------------------------------------
                // Methods

                async void LoadSampleData()
                {
                    // Status = 0 means task is not done and Status = 1 means it's done
                    Todo testData4 = new Todo
                    {
                        Title = "Finish Chapter 5",
                        Due_Date = Convert.ToDateTime("2023-01-07"),
                        Status = 0,
                        Project = "Math"
                    };
                    context.Todoes.Add(testData4);
                    Todo testData1 = new Todo
                    {
                        Title = "Finish Chapter 2",
                        Due_Date = Convert.ToDateTime("2022-12-19"),
                        Status = 1,
                        Project = "Math"
                    };
                    context.Todoes.Add(testData1);
                    Todo testData3 = new Todo
                    {
                        Title = "Finish Chapter 4",
                        Due_Date = Convert.ToDateTime("2022-12-30"),
                        Status = 0,
                        Project = "Math"
                    };
                    context.Todoes.Add(testData3);
                    Todo testData2 = new Todo
                    {
                        Title = "Finish Chapter 3",
                        Due_Date = Convert.ToDateTime("2022-12-25"),
                        Status = 0,
                        Project = "Math"
                    };
                    context.Todoes.Add(testData2);
                    context.SaveChanges();
                }

                void MarkTaskAsDone()
                {
                    int id;
                    do
                    {
                        do
                        {
                            Console.WriteLine("Input the task ID you would like to mark as done");
                            try
                            {
                                id = Int32.Parse(Console.ReadLine());
                                break;
                            }
                            catch (FormatException e) { Console.WriteLine(e.Message); Console.Write("Try again."); }
                            catch (Exception e) { Console.WriteLine(e.Message); Console.Write("Try again."); }

                        } while (true);

                        Console.Clear();
                        var foundTask = context.Todoes.FirstOrDefault(x => x.Id == id);

                        if (foundTask == null)
                        {
                            Console.WriteLine($"ID {id} could not be found in the database. Try again.");
                        }
                        else
                        {
                            WriteCategoriesInConsole();
                            OutputToDoList(foundTask);
                            EndOfToDoList();
                        }

                        // integer value of 0 indicates task is not done, 1 indicates it is done.
                        byte status;
                        Console.WriteLine("Type the new status of the task. 0 means task is not done yet, 1 means task is done.");
                        do
                        {
                            try
                            {
                                status = Byte.Parse(Console.ReadLine());
                                if (status == 0 || status == 1) { break; }
                                else { Console.WriteLine("Try again. Only \"1\" and \"2\" are acceptable inputs."); }
                            }
                            catch (FormatException e) { Console.WriteLine(e.Message); }
                            catch (Exception e) { Console.WriteLine(e.Message); }

                        } while (true);

                        Console.WriteLine("Task has been marked as done. Well done!");
                        foundTask.Status = status;
                        context.SaveChanges();
                        break;

                    } while (true);
                }

                void DeleteTask()
                {
                    int id;
                    do
                    {
                        do
                        {
                            Console.WriteLine("Input the task ID you would like to delete");
                            try
                            {
                                id = Int32.Parse(Console.ReadLine());
                                break;
                            }
                            catch (FormatException e) { Console.WriteLine(e.Message); Console.Write("Try again."); }
                            catch (Exception e) { Console.WriteLine(e.Message); Console.Write("Try again."); }

                        } while (true);

                        Console.Clear();
                        var foundTask = context.Todoes.FirstOrDefault(x => x.Id == id);

                        if (foundTask == null)
                        {
                            Console.WriteLine($"ID {id} could not be found in the database. Try again.");
                        }
                        else
                        {
                            WriteCategoriesInConsole();
                            OutputToDoList(foundTask);
                            EndOfToDoList();

                            do
                            {
                                Console.WriteLine("Are you sure you wish to delete the task shown above? Type \"Yes\" or \"No\"");
                                string choice = Console.ReadLine();

                                if (choice.ToLower().Equals("yes"))
                                {
                                    context.Todoes.Remove(foundTask);
                                    context.SaveChanges();
                                    Console.WriteLine($"Task with the ID {id} has been deleted.");
                                    Console.WriteLine("");
                                    break;
                                }
                                else if (choice.ToLower().Equals("no"))
                                {
                                    Console.WriteLine("Cancelling the current operation. Task will NOT be deleted.");
                                    Console.WriteLine("");
                                    break;
                                }
                            } while (true);
                            break;
                        }
                    } while (true);
                }

                bool UpdateTaskCheckForValidString(string input)
                {
                    if (input == null) { return false; }
                    else if (input.Length <= 2) { return false; }
                    else { return true; }
                }

                string AddNewTaskStringValidation()
                {
                    string input;
                    do
                    {
                        try
                        {
                            input = Console.ReadLine();

                            if (input == null || input.Length < 2)
                            {
                                Console.WriteLine("Incorrect input. Please try again.");
                                Console.WriteLine();
                            }
                            else
                            {
                                break;
                            }

                        }
                        catch (Exception e) { Console.WriteLine($"Exception error: {e.Message}"); }
                    } while (true);
                    Console.WriteLine("");
                    return input;
                }

                void OutputToDoList(Todo task)
                {
                    if (task.Status == 0)
                    {
                        Console.Write(task.Id.ToString().PadRight(5) + task.Title.PadRight(25) + task.Due_Date.ToShortDateString().PadRight(15));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Not done".PadRight(15));
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(task.Project);
                    }
                    else if (task.Status == 1)
                    {
                        Console.Write(task.Id.ToString().PadRight(5) + task.Title.PadRight(25) + task.Due_Date.ToShortDateString().PadRight(15));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Not done".PadRight(15));
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(task.Project);
                    }
                    Console.WriteLine("");
                }

                void WriteCategoriesInConsole()
                {
                    Console.Clear();
                    Console.WriteLine("ID".PadRight(5) + "Title".PadRight(25) + "Due Date".PadRight(15) + "Status".PadRight(15) + "Project");
                    Console.WriteLine("---".PadRight(5) + "---".PadRight(25) + "---".PadRight(15) + "---".PadRight(15) + "---");
                }

                void EndOfToDoList()
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");
                }

                List<Todo> GatherDBData()
                {
                    int i = 1;
                    List<Todo> todoListFromDB = new List<Todo>();
                    int totalNumberOfTasks = context.Todoes.Count();
                    int foundNumberOfTasks = 0;

                    do
                    {
                        // Attempts to find the todo tasks by their id, starting from 1.
                        Todo tempTask = context.Todoes.FirstOrDefault(x => x.Id == i);
                        // If no task could be found and the program have iterated equal times to the total number of tasks,
                        // the logic will break.
                        if (tempTask == null && foundNumberOfTasks == totalNumberOfTasks)
                        {
                            break;
                        }
                        else if (tempTask == null)
                        {
                            i++;
                        }
                        else
                        {
                            todoListFromDB.Add(tempTask);
                            i++;
                            foundNumberOfTasks++;
                        }
                    } while (true);
                    return todoListFromDB;
                }

                void AddNewTask()
                {
                    Console.Clear();
                    Console.WriteLine("You will now be adding a new task. Start by typing the title of the task.");
                    string title = AddNewTaskStringValidation();

                    DateTime dueDate;
                    Console.WriteLine("Type the due date in the following format yyyy-MM-dd ");
                    do
                    {
                        string line = Console.ReadLine();

                        // Checks if user's input of due date is correct. If not, prompts user to retry again.
                        DateTime dt;
                        if (DateTime.TryParseExact(line, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            dueDate = dt;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid date, please retry");
                        }
                    } while (true);

                    // integer value of 0 indicates task is not done, 1 indicates it is done.
                    byte status;
                    Console.WriteLine("Type the status of the task. 0 means task is not done yet, 1 means task is done.");
                    do
                    {
                        try
                        {
                            status = Byte.Parse(Console.ReadLine());
                            if (status == 0 || status == 1) { break; }
                            else { Console.WriteLine("Try again. Only \"1\" and \"2\" are acceptable inputs."); }
                        }
                        catch (FormatException e) { Console.WriteLine(e.Message); }
                        catch (Exception e) { Console.WriteLine(e.Message); }

                    } while (true);

                    Console.WriteLine("Type the name of the project.");
                    string projectName = AddNewTaskStringValidation();

                    Todo task = new Todo
                    {
                        Title = title,
                        Due_Date = dueDate,
                        Status = status,
                        Project = projectName
                    };

                    context.Todoes.Add(task);
                    context.SaveChanges();
                    Todo foundTask = context.Todoes.FirstOrDefault(x => x.Title == title && x.Due_Date == dueDate && x.Status == status && x.Project == projectName);

                    Console.WriteLine($"Task with the id {foundTask.Id} has been added.");
                    Console.WriteLine("");
                }

                void UpdateTask()
                {

                    // For updating a task
                    string newTitle = "";
                    DateTime newDueDate;
                    byte newStatus;
                    string newProjectName = "";
                    int id;

                    do
                    {
                        do
                        {
                            Console.WriteLine("Input the task ID you would like to change.");
                            try
                            {
                                id = Int32.Parse(Console.ReadLine());
                                break;
                            }
                            catch (FormatException e) { Console.WriteLine(e.Message); Console.Write("Try again."); }
                            catch (Exception e) { Console.WriteLine(e.Message); Console.Write("Try again."); }

                        } while (true);

                        Console.Clear();
                        Todo foundTask = context.Todoes.FirstOrDefault(x => x.Id == id);

                        if (foundTask == null)
                        {
                            Console.WriteLine($"ID {id} could not be found in the database. Try again.");
                        }
                        else
                        {
                            WriteCategoriesInConsole();
                            OutputToDoList(foundTask);
                            EndOfToDoList();

                            Console.WriteLine($"Type in a new title for the task or click enter to keep current type \"{foundTask.Title}\"");
                            do
                            {
                                newTitle = Console.ReadLine();
                                Console.WriteLine();
                                if (newTitle.Equals("")) { newTitle = foundTask.Title; break; }
                            } while (!UpdateTaskCheckForValidString(newTitle));

                            Console.WriteLine($"Type in a new due date for the task or click enter to keep current due date \"{foundTask.Due_Date}\"");
                            do
                            {
                                string line = Console.ReadLine();

                                // Checks if user's input of due date is correct. If not, prompts user to retry again.
                                DateTime dt;
                                if (line.Equals("")) { newDueDate = foundTask.Due_Date; }
                                else if (DateTime.TryParseExact(line, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dt))
                                {
                                    newDueDate = dt;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid date, please retry");
                                }
                            } while (true);

                            Console.WriteLine($"Type in a new status for the task || Keep in mind 0 = Not done and 1 = Done");
                            do
                            {
                                try
                                {
                                    newStatus = Byte.Parse(Console.ReadLine());
                                    Console.WriteLine();
                                    if (newStatus.Equals(null) || newStatus.ToString() == null) { newStatus = foundTask.Status; break; }
                                    else if (newStatus == 0 || newStatus == 1) { break; }
                                    else { Console.WriteLine("Invalid status, try again."); }
                                }
                                catch (FormatException e) { Console.WriteLine(e.Message); }
                                catch (Exception e) { Console.WriteLine(e.Message); }
                            } while (true);


                            Console.WriteLine($"Type in a new project name for the task or click enter to keep current name \"{foundTask.Project}\"");
                            do
                            {
                                newProjectName = Console.ReadLine();
                                Console.WriteLine();
                                if (newProjectName.Equals("")) { newProjectName = foundTask.Project; break; }
                            } while (!UpdateTaskCheckForValidString(newProjectName));

                            foundTask.Title = newTitle;
                            foundTask.Due_Date = newDueDate;
                            foundTask.Status = newStatus;
                            foundTask.Project = newProjectName;

                            context.SaveChanges();
                            Console.WriteLine("Task has been updated.");
                            EndOfToDoList();
                            break;
                        }
                    } while (true);
                }
            }
        }
    }
}
