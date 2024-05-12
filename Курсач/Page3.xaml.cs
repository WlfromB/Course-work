using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;


namespace Курсач
{


    /// <summary>
    /// Логика взаимодействия для Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        delegate void checkerCount(int count);

        event checkerCount IsAllLessonsSetted;

        private int maxLesInDay;

        public int MaxLesInDay
        {
            get { return maxLesInDay; }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentException("Value can`t be lower than zero!");
                }
                else
                {
                    maxLesInDay = value;
                }
            }
        }

        private string _DayOfWeek;

        public string DayOfWeek
        {
            get
            {
                return _DayOfWeek;
            }
            private set
            {
                _DayOfWeek = value;
            }
        }

        private static DayOfWeek day;

        public static DayOfWeek Day
        {
            get { return day; }
            private set { day = value; }
        }

        private static int countLessonsInDayCur = 0;

        public static int countLessonsInDay;

        public Page3(string _dayOfWeek)
        {
            DayOfWeek = _dayOfWeek;
            InitializeComponent();            
            IsAllLessonsSetted += CheckIsAllSetted;
            Mouse.OverrideCursor = Cursors.Wait;
            Day = GetDayOfWeek(DayOfWeek.GetEngDay(), out int maxLessonInDay);
            MaxLesInDay = maxLessonInDay;
            countLessonsInDay = CalcLessonsInDay();
            FormUniform(Day.LessonsForClasses, DayOfWeek);
            //автоматическое составление
            FormUniformSchedule((grid.Children[1] as UniformGrid), MaxLesInDay);
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        //ее надо применить при формировании дня
        private bool IsAnyConflict(int lessonsInDay, List<LessonsForTeacher> forTeachers, out int index)
        {
            //int lessonsInDay = Day.LessonsForClasses[0].Times.Length;// беру первый элемент в списке классов,
                                                                     // у него нахожу длину массива времен,
                                                                     // могу так сделать потому что они все выравнены
            index = -1;
            foreach(LessonsForTeacher item in forTeachers)
            {
                if(item.CheckerTime.Length > lessonsInDay)
                {
                    index = forTeachers.IndexOf(item);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// это будет список учителей, предлагаемых для замены в этот день конфликтного преподавателя.
        /// 
        ///если список будет пустой, то отправляем запрос на удаление последнего урока у конфликтного преподавателя
        /// </summary>
        /// <param name="teachers">пришедший с API список преподавателей</param>
        /// <param name="conflictTeacher">конфликтный преподаватель</param>
        /// <param name="lessonInDay">максимальное количество уроков в составляемом дне</param>
        /// <param name="indexChangeConflict">индекс замены - урок по порядку в списке у конфликтного преподавателя</param>
        /// <returns></returns>

        private List<LessonsForTeacher> FormListBufferTeachers(List<LessonsForTeacher> teachers, LessonsForTeacher conflictTeacher, int lessonInDay ,out int indexChangeConflict)
        {
            List<LessonsForTeacher> bufferTeachers = new List<LessonsForTeacher>();
            indexChangeConflict = -1;
            foreach (LessonsForTeacher item in teachers) //  сформировали тех у кого есть хотя бы 1 место,
                                                        //  вне зависимости от того нужны ли конкретно их услуги по замещению нам сейчас
            {
                if(item.CheckerTime.Length < lessonInDay)
                {
                    bufferTeachers.Add(item);
                }
            }

            for(int i = 0; i < conflictTeacher.CheckerTime.Length; i++)
            {
                string currentSubject = conflictTeacher.LessonAndClass[i].Item1;
                List<LessonsForTeacher> nextIteration = new List<LessonsForTeacher>();
                foreach(LessonsForTeacher item in bufferTeachers)
                {
                    foreach(Tuple<string,string> lessonAndClass in item.LessonAndClass)
                    {
                        if(lessonAndClass.Item1 == currentSubject)
                        {
                            nextIteration.Add(item);
                            break;
                        }
                    }
                }
                if(nextIteration.Count != 0)
                {
                    indexChangeConflict = i;
                    bufferTeachers = nextIteration;
                    break;
                }
            }            
            return bufferTeachers;
        }


        // автоматическое составление расписания
        private void FormUniformSchedule(UniformGrid uniform, int maxLessonsInDay)
        {
            // у меня есть возможность смотреть занят ли уже в этой клетке учитель
            // мне нужно имя класса и время. буду брать время сквозным
            for(int j = 0; j < Day.LessonsForClasses.Count; j++)
            {
                LessonsForClass classes = Day.LessonsForClasses[j]; 
                for (int i = 0; i < maxLessonsInDay; i++)
                {
                    if (IsCanInsertOnTimePosition(i, classes.Name, Day.LessonsFroEachTeacher, classes.Lessons, out string lesson))
                    {                        
                        countLessonsInDayCur++;
                        CheckIsAllSetted(countLessonsInDayCur);
                        // вставить урок в uniform, 
                        // удалить урок из списка уроков у класса
                        Grid classGrid = uniform.Children[j] as Grid; // уровень класса
                        Grid rowInClassGrid = classGrid.Children[i+1] as Grid;
                        Label placeForNameLesson = rowInClassGrid.Children[0] as Label;
                        Button actionsWithLesson = rowInClassGrid.Children[1] as Button;
                        actionsWithLesson.Click -= Click_Add; // отвязываем обработчик добавления
                        actionsWithLesson.Click += Click_Del; // привязываем обработчик удаления
                        actionsWithLesson.Content = "X"; // меняем иконку
                        placeForNameLesson.Content = new TextBlock()
                        {
                            Text = lesson,
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontFamily = new FontFamily("Arial"),
                            FontSize = 15
                        };
                        classes.Lessons.Remove(lesson);
                        if(classes.Lessons.Count == 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Grid classGrid = uniform.Children[j] as Grid; // уровень класса
                        Grid rowInClassGrid = classGrid.Children[i + 1] as Grid;
                        Label placeForNameLesson = rowInClassGrid.Children[0] as Label;
                        placeForNameLesson.Content = new TextBlock()
                        {
                            Text = "",
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontFamily = new FontFamily("Arial"),
                            FontSize = 15
                        };
                    }
                }
            }
            
        }

        /// <summary>
        /// Внутри этой функции уже происходит вставка времени преподавателю,
        /// out параметр - урок, который удалось вставить, если не удалось вернется пустая строка
        /// </summary>
        /// <param name="time"></param>
        /// <param name="_class"></param>
        /// <param name="forTeachers"></param>
        /// <param name="lessons"></param>
        /// <param name="lesson"></param>
        /// <returns></returns>
        private static bool IsCanInsertOnTimePosition(int time, string _class, List<LessonsForTeacher> forTeachers, List<string> lessons, out string lesson)
        {
            foreach(string item in lessons)
            {
                LessonsForTeacher insertedonTeacher = forTeachers.FindTeacher(item, _class);
                if(insertedonTeacher != null)
                {
                    if (insertedonTeacher.CheckerTime[time] == 0)
                    {
                        insertedonTeacher.FindLessonAdd(item,_class, time);
                        lesson = item;
                        return true;
                    }
                }
            }
            lesson = "";
            return false;
        }
        /// <summary>
        /// забирает урок по указанному индексу у конфликтного преподавателя и передает его временному
        /// </summary>
        /// <param name="conflictTeacher">преподаватель с уроком, который невозможно разместить</param>
        /// <param name="indexConflictLesson">индекс заменяемого урока</param>
        /// <param name="whichHelpTeacher">преподаватель которому отдают урок</param>
        private void ChangeTeachersLesson(LessonsForTeacher conflictTeacher, int indexConflictLesson, LessonsForTeacher whichHelpTeacher)
        {
            Tuple<string, string> lesson = conflictTeacher.LessonAndClass[indexConflictLesson];
            conflictTeacher.LessonAndClass.Remove(lesson);
            conflictTeacher.CheckerTime = new int[conflictTeacher.CheckerTime.Length - 1];
            whichHelpTeacher.LessonAndClass.Add(lesson);
            whichHelpTeacher.CheckerTime = new int[whichHelpTeacher.CheckerTime.Length + 1];
        }

        public int CalcLessonsInDay()
        {
            int result = 0;
            foreach(LessonsForClass item in Day.LessonsForClasses)
            {
                result += item.Lessons.Count;
            }
            return result;
        }


        private int IndexChildren(string childName)
        {
            for(int i = 0;i<grid.Children.Count;i++)
            {
                if (grid.Children[i] is FrameworkElement element && element.Name == childName)
                {
                    return i;
                }
            }
            return -1;
        }

        private void CheckIsAllSetted(int count)
        {
            if(count  == countLessonsInDay)
            {
                grid.Children.RemoveAt(IndexChildren("bar"));
                StackPanel panelForSave = new StackPanel() 
                { 
                    Orientation = Orientation.Horizontal, 
                    Name = "panelForSave"
                };
                Label labelForNameDay = new Label()
                {
                    Content = new TextBlock()
                    {
                        Text = _DayOfWeek,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 30
                    },
                    Margin = new Thickness(100, 0, 100,0)
                };
                Button buttonForSave = new Button()
                {
                    Content = new TextBlock()
                    { Text = "Сохранить",
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 30
                    },
                    Margin = new Thickness(300, 0, 100, 0),                  
                    
                };
                buttonForSave.Click += ButtonSave_Click;
                AddHandToButton(buttonForSave);
                panelForSave.Children.Add(labelForNameDay);
                panelForSave.Children.Add(buttonForSave);
                Grid.SetColumn(panelForSave, 0);
                Grid.SetRow(panelForSave, 0);
                grid.Children.Add(panelForSave);
                IsAllLessonsSetted -= CheckIsAllSetted;
                IsAllLessonsSetted += CheckIsNotAllSetted;
            }
        }
        private void AddHandToButton(Button button)
        {
            button.MouseMove += (sender, e) =>
            {
                if (button.IsMouseOver)
                {
                    button.Cursor = Cursors.Hand;
                }
                else
                {
                    button.Cursor = Cursors.Arrow;
                }
            };
        }
        /// сформировать объект отправляемый в базу данных . Нужен класс с полями id учителя, поле имя класса, название предмета, номер времени
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            int indexUniformInGrid = IndexChildren("uniformGrid");

            Schedule schedule = new Schedule();
            schedule.FormShedule(grid.Children[indexUniformInGrid] as UniformGrid, DayOfWeek.GetEngDay());

            JArray jsonForPost = schedule.FormJSON();
            try
            {
                PostSchedule(ApiMethods.POST, jsonForPost);

                MessageBox.Show("Добавление произошло!");

                NavigationService.Navigate(new Page1());
            }
            catch (Exception ex)
            {

            }

        }

        private void PostSchedule(string method, JArray jsonForPost)
        {            
            string addQuery = $"{method}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonForPost), Encoding.UTF8, "application/json");                    
                    HttpResponseMessage response = client.PostAsync(new Uri(ExtensionMethods.pathConnection, addQuery).ToString(),content).Result;
                    response.EnsureSuccessStatusCode();                    
                }
                catch (Exception e) { }
            }
        }


        /// удаление панели и добавление заголовка дня
        private void CheckIsNotAllSetted(int count)
        {
            if(count < countLessonsInDay)
            {
                grid.Children.RemoveAt(IndexChildren("panelForSave"));
                Label labelDay = new Label()
                {
                    Content = new TextBlock()
                    {
                        Text = _DayOfWeek,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 30
                    },
                    Name = "bar"
                };
                Grid.SetColumn(labelDay, 0);
                Grid.SetRow(labelDay, 0);
                grid.Children.Add(labelDay);
                IsAllLessonsSetted -= CheckIsNotAllSetted;
                IsAllLessonsSetted += CheckIsAllSetted;
            }
        }
        private void FormBar(string day)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.15, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.85, GridUnitType.Star) });
            TextBlock bar = new TextBlock();
            bar.Text = day;
            bar.HorizontalAlignment = HorizontalAlignment.Center;
            bar.VerticalAlignment = VerticalAlignment.Center;
            bar.FontFamily = new FontFamily("Arial");
            bar.FontSize = 30;
            bar.Name = "bar";
            Grid.SetColumn(bar, 0);
            Grid.SetRow(bar, 0);
            grid.Children.Add(bar);
        }

        private void FormUniform(List<LessonsForClass> forClasses, string day) 
        {
            FormBar(day);
            UniformGrid _uniformGrid = new UniformGrid();
            _uniformGrid.Name = "uniformGrid";
            int magicNumber = 16;
            if (forClasses.Count > magicNumber)
            {
                _uniformGrid.Columns = magicNumber;
                for (int i = 0; i < forClasses.Count; i += _uniformGrid.Columns - 1)
                    _uniformGrid.Rows += 1;
            }
            else
            {
                _uniformGrid.Columns = forClasses.Count;
                _uniformGrid.Rows = 1;
            }
            FormGrids(_uniformGrid, forClasses, magicNumber);
            Grid.SetRow(_uniformGrid, 1);
            grid.Children.Add(_uniformGrid);
        }
        //в зависимости от команды подставить верный контент
        private object CreateContent(Button btn)
        {
            if (ExtensionMethods.GetNumberTime(btn.Name) !=0)
                return "X";
            return "+";
        }
        // Добавить обновление цвета для случая красного лэйбла
        private void Click_Del(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid tempGrid = button.Parent as Grid;
            Label label = tempGrid.Children[0] as Label; // получили имя урока
            string nameLesson = GetLabelContent(label);//(string)label.Content;
            tempGrid = tempGrid.Parent as Grid; // поднялись на уровень класса
            int number = Day.LessonsForClasses.Count > 16 ? 16 : Day.LessonsForClasses.Count; // определим число элементов в строке
            number = ExtensionMethods.GetNumberClass(button.Name, number); // получим номер класса с которым мы сейчас работаем
            Day.LessonsForClasses[number].Lessons.Add(GetLabelContent(label));// (string)label.Content); // в нужный список положим удаляемый класс
            label.Content = string.Empty; // чистим имя под класс
            label.Background = grid.Background; // убираем красный цвет
            int time = ExtensionMethods.GetNumberTime(button.Name); // получаем время с которым будем работать
            Day.LessonsForClasses[number].Times[time] = 0; // устанавливаем нужному классу значение времени свободным
            tempGrid = tempGrid.Children[0] as Grid; // получаем сетку где хранится имя класса
            label = tempGrid.Children[0] as Label; //  получаем имя класса
            LessonsForTeacher forTeacher = null; // заводим переменную под удаление
            foreach (var item in Day.LessonsFroEachTeacher)
            {
                forTeacher = item.FindLessonRemove(nameLesson, GetLabelContent(label), time);//(string)label.Content, time); // ищем нужного учителя
                if (forTeacher != null)
                {                    
                    tempGrid = tempGrid.Parent as Grid; // поднимаемся на уровень сетки класса
                    UniformGrid uniform = tempGrid.Parent as UniformGrid;
                    for (int i = 0;i< uniform.Children.Count;i++)
                    {
                        Grid childGrid = uniform.Children[i] as Grid; // получаем i-ый класс
                        Grid chtobiDostatClass = childGrid.Children[0] as Grid; // получаем сетку где хранится имя класса
                        Label lbl1 = chtobiDostatClass.Children[0] as Label; // получаем объект - класс
                        childGrid = childGrid.Children[time + 1] as Grid; // падаем в сетку где хранится предмет
                        Label lbl2 = childGrid.Children[0] as Label; // получаем объект - строка нужная по времени хранящая предмет
                        LessonsForTeacher teacher = Day.LessonsFroEachTeacher.FindTeacher(GetLabelContent(lbl2),
                            GetLabelContent(lbl1));//(string)lbl2.Content, (string)lbl1.Content); // находим учителя этого предмета в этом классе
                        if (teacher != null && teacher.Name == forTeacher.Name) // если такой учитель есть то сравниваем с тем у которого выявили ошибку
                        {
                            lbl2.Background = grid.Background; // убираем красный цвет
                            IsAllLessonsSetted += CheckIsAllSetted;
                        }

                    }
                    
                }
            }

            button.Click -= Click_Del;
            button.Click += Click_Add;
            button.Content = "+";
            countLessonsInDayCur--;
            IsAllLessonsSetted?.Invoke(countLessonsInDayCur);
        }


        private void Click_Add(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid tempGrid = button.Parent as Grid; // взяли строку 
            int parameter = ExtensionMethods.GetNumberClass(button.Name, Day.LessonsForClasses.Count > 16 ? 16 : Day.LessonsForClasses.Count); // получаем номер в списке объектов классов - класс с которым будем работать(формируемый)
            TaskWindow taskWindow = new TaskWindow(Day.LessonsForClasses[parameter]); // инициализируем окно выбора предмета
            tempGrid = tempGrid.Parent as Grid; // взяли класс ( имя + строки где будут заполняться предметы ) 
            tempGrid = tempGrid.Children[0] as Grid; // спустились на один уровень в ячейку где хранится имя класса
            Label label = tempGrid.Children[0] as Label; // в ячейке взяли и запомнили имя класса
            taskWindow.Title = GetLabelContent(label);//(string)label.Content; // сменили название окна на имя класса
            tempGrid = button.Parent as Grid; // взяли строку
            label = tempGrid.Children[0] as Label; // запомнили объект - формируемый предмет
            taskWindow.Owner = Window.GetWindow(this); // пришили доп окно к исходному
            taskWindow.ShowDialog(); // объявили модальное окно
            label.Content = taskWindow.Lesson.ToString(); // вернули выбранный элемент из списка 
            LessonsForTeacher forTeacher = null; // временная переменная нужная для работы с выявлением ошибок составления
            int time = ExtensionMethods.GetNumberTime(button.Name); // время с которым мы будем работать
            foreach (var item in Day.LessonsFroEachTeacher)
            {
                forTeacher = item.FindLessonAdd(GetLabelContent(label),
                    taskWindow.Title, time);//(string)label.Content, taskWindow.Title, time); // получаем либо null либо объект
                if (forTeacher != null) 
                    break; // если объект найден то выйдем из цикла
            }
            if (!forTeacher.IsCorrectAddition(time)) // проверка наличия ошибки по времени
            {
                label.Background = new SolidColorBrush(Colors.Red); // окрашиваем текущую ячейку в красный цвет - пометка ошибки
                tempGrid = tempGrid.Parent as Grid; // поднимаемся на уровень класса
                UniformGrid uniform = tempGrid.Parent as UniformGrid;// поднимаемся на уровень списка классов
                for (int i = 0; i < uniform.Children.Count; i++) // цикл по всем элементам в в списке классов - уже временно составленных
                {
                    Grid childGrid = uniform.Children[i] as Grid; // получаем i-ый класс
                    Grid chtobiDostatClass = childGrid.Children[0] as Grid; // получаем сетку где хранится имя класса
                    Label lbl1 = chtobiDostatClass.Children[0] as Label; // получаем объект - класс
                    childGrid = childGrid.Children[time + 1] as Grid; // падаем в сетку где хранится предмет
                    Label lbl2 = childGrid.Children[0] as Label; // получаем объект - строка нужная по времени хранящая предмет
                    LessonsForTeacher teacher = Day.LessonsFroEachTeacher.FindTeacher(
                        GetLabelContent(lbl2), GetLabelContent(lbl1)
                        );//(string)lbl2.Content, (string)lbl1.Content); // находим учителя этого предмета в этом классе
                    if (teacher != null && teacher.Name == forTeacher.Name) // если такой учитель есть то сравниваем с тем у которого выявили ошибку
                    {
                        lbl2.Background = new SolidColorBrush(Colors.Red); // если это один и тот же человек то ошибка найдена - красим строку
                        IsAllLessonsSetted -= CheckIsAllSetted;
                    }
                }
            }
            Day.LessonsForClasses[parameter].Times[time] = 1; // показываем что место заполнено
            button.Click-=Click_Add; // отвязываем обработчик добавления
            button.Click += Click_Del; // привязываем обработчик удаления
            button.Content = "X"; // меняем иконку
            countLessonsInDayCur++;            
            IsAllLessonsSetted?.Invoke(countLessonsInDayCur);
        }
        
        private Grid FormChildGrid(int rowUnif, int colUnif, int row)// row - номер строки в uniform , col - номер столбца
        {
            Label label = new Label()
            {
                Content = string.Empty,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Arial"),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
            };
            Grid grid = new Grid();
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);
            if (row != 0)
            {
                Button btn = new Button();
                btn.Name = ExtensionMethods.GetNameBtn(rowUnif, colUnif, row - 1);
                btn.Click += Click_Add;
                btn.Content = "+";// - зависит от команды!!! надо обработку 
                Grid.SetColumn(btn, 1);
                /* btn.CommandBindings.Add(command);
                 btn.Command = routed; // - нужно добавить конкретную команду*/
                // Нужно еще добавить команду/событие                 
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.85, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.15, GridUnitType.Star) });                
                grid.Children.Add(btn);
            }
            return grid;
        }

        private void FormGrid(int row, int col, UniformGrid uniform, List<LessonsForClass> forClasses, int magicNumber)
        {
            Grid grid = new Grid();                 
            LessonsForClass lessonsForClass = forClasses.FindByRowCol(row, col, magicNumber);
            for (int i = 0; i <= lessonsForClass.Times.Length; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1, GridUnitType.Star) });
            }
            Grid childGrid = FormChildGrid(row, col, 0 );// - имя класса + кнопка добавления
            Grid.SetRow(childGrid, 0);
            grid.Children.Add(childGrid);
            Label uIElement = (Label)childGrid.Children[0];
            uIElement.Content = lessonsForClass.Name;
            for (int i = 1;i< grid.RowDefinitions.Count;i++)
            {
                Grid chGrid = FormChildGrid(row, col, i);
                Grid.SetRow(chGrid, i);
                grid.Children.Add(chGrid);
            }
            grid.Name = $"grid{ExtensionMethods.Get2Numeric(row)}{ExtensionMethods.Get2Numeric(col)}";
            uniform.Children.Add(grid);
        }

        private void FormGrids(UniformGrid uniformGrid, List<LessonsForClass> forClasses, int magicNumber)
        {
            for(int i = 0;i< uniformGrid.Rows;i++)
            {
                for(int j = 0; j < uniformGrid.Columns && (i*magicNumber + j) < forClasses.Count; j++)
                {
                    FormGrid(i, j , uniformGrid , forClasses, magicNumber);
                }
            }
            
        }


        private DayOfWeek GetDayOfWeek(string day, out int number)
        {
            List<LessonsForClass> forClasses = LessonsForClass.
                FormLesForEachClass(ExtensionMethods.ReadDataAsync(ApiMethods.GETClasses, day));
            string data = ExtensionMethods.ReadDataAsync(ApiMethods.GETTeachers, day);
            List<LessonsForTeacher> forTeachers = LessonsForTeacher.FormLesForEachTeacher(data);
            number = forClasses.CorrectionListClasses();
            while(IsAnyConflict(number, forTeachers, out int index))
            {
                // обработать сценарии пользователем
                List<LessonsForTeacher> buffer = FormListBufferTeachers(forTeachers, forTeachers[index], number, out int indexChangeConflict);
                // нужно написать метод который будет заменять уроки у преподавателей по выбранным параметрам
                // выбранные параметры: 
                // 1) предмет который мы можем заменить с помощью буфферных преподавателей
                // 2) преподаватель, которого выбрали среди буфферных

                ConflictResolverWindow conflictResolver = new ConflictResolverWindow(buffer, forTeachers[index], indexChangeConflict);
                if(conflictResolver.Selected != -1)
                {
                    ChangeTeachersLesson(forTeachers[index], indexChangeConflict, buffer[conflictResolver.Selected]);
                }
                else
                {
                    forTeachers[index].LessonAndClass.Remove(forTeachers[index].LessonAndClass[indexChangeConflict]);
                    forTeachers[index].CheckerTime = new int[forTeachers[index].CheckerTime.Length + 1];
                }
            }
            forTeachers.CorrectionListTeachers(number);
            return new DayOfWeek(day, forClasses, forTeachers);
        }


        public static string GetLabelContent(Label label)
        {
            if (label.Content is string stringContent)
            {
                return stringContent;
            }
            if(label.Content is TextBlock textblock)
            {
                return textblock.Text;
            }
            return string.Empty;
        }      

    }
}
