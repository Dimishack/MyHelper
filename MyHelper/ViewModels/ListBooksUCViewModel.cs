using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Models.Books;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListBooksUCViewModel(IUserDialog userDialog, IWorkWithJSONFile workWithJSON, IOpenWindows openWindows) : ViewModel
    {
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IWorkWithJSONFile _workWithJSON = workWithJSON;
        private readonly IOpenWindows _openWindows = openWindows;
        private bool _isFirstLoad = true;
        #region Properties...

        #region ListBooks : BindingList<MyBook> - Список книг

        ///<summary>Список книг</summary>
        private BindingList<MyBook> _listBooks = [];

        ///<summary>Список книг</summary>
        public BindingList<MyBook> ListBooks { get => _listBooks; set => Set(ref _listBooks, value); }

        #endregion

        #region SelectedBook : MyBook? - Выбранная книга

        ///<summary>Выбранная книга</summary>
        private MyBook? _selectedBook;

        ///<summary>Выбранная книга</summary>
        public MyBook? SelectedBook { get => _selectedBook; set => Set(ref _selectedBook, value); }

        #endregion

        #region ListBooksOnReading : IList<MyBook> - COMMENT

        ///<summary>COMMENT</summary>
        private BindingList<MyBook> _listBooksOnReading = [];

        ///<summary>COMMENT</summary>
        public BindingList<MyBook> ListBooksOnReading { get => _listBooksOnReading; set => Set(ref _listBooksOnReading, value); }

        #endregion

        #endregion

        #region Commands...

        #region LoadedWindowCommand - Команда - загрузка окна

        ///<summary>Команда - загрузка окна</summary>
        private ICommand? _loadedWindowCommand;

        ///<summary>Команда - загрузка окна</summary>
        public ICommand LoadedWindowCommand => _loadedWindowCommand
            ??= new LambdaCommandAsync(OnLoadedWindowCommandExecuted, CanLoadedWindowCommandExecute);

        ///<summary>Проверка возможности выполнения - загрузка окна</summary>
        private bool CanLoadedWindowCommandExecute(object? p) => _isFirstLoad;

        ///<summary>Логика выполнения - загрузка окна</summary>
        private async Task OnLoadedWindowCommandExecuted(object? p)
        {
            BindingList<MyBook>? books;
            if ((books = await _workWithJSON.ReadFileAsync<BindingList<MyBook>>(@"Data/Books.json")) is not null)
            {
                ListBooks = books;
                ListBooksOnReading = new(_listBooks.Where(b => b.IsReading).ToList());
                ((Command)SaveBooksCommand).Executable = false;
            }
            ListBooks.ListChanged += ListBooks_ListChanged;
            _isFirstLoad = false;
        }


        #endregion

        #region AddBookCommand - Команда - добавить книгу

        ///<summary>Команда - добавить книгу</summary>
        private ICommand? _addBookCommand;

        ///<summary>Команда - добавить книгу</summary>
        public ICommand AddBookCommand => _addBookCommand
            ??= new LambdaCommand(OnAddBookCommandExecuted);

        ///<summary>Логика выполнения - добавить книгу</summary>
        private void OnAddBookCommandExecuted(object? p)
        {
            var book = new MyBook();
            if (!_openWindows.OpenCreator_EditorBookWindow(book, "Добавить книгу")) return;

            ListBooks.Add(book);
            OnPropertyChanged(nameof(ListBooks));
            _userDialog.InformationMessage("Книга добавлена");
        }

        #endregion

        #region EditBookCommand - Команда - редактировать книгу

        ///<summary>Команда - редактировать книгу</summary>
        private ICommand? _editBookCommand;

        ///<summary>Команда - редактировать книгу</summary>
        public ICommand EditBookCommand => _editBookCommand
            ??= new LambdaCommand<MyBook?>(OnEditBookCommandExecuted, CanEditBookCommandExecute);

        ///<summary>Проверка возможности выполнения - редактировать книгу</summary>
        private bool CanEditBookCommandExecute(MyBook? p) => p is not null;

        ///<summary>Логика выполнения - редактировать книгу</summary>
        private void OnEditBookCommandExecuted(MyBook? p)
        {
            int index = _listBooksOnReading.IndexOf(p!);
            if (!_openWindows.OpenCreator_EditorBookWindow(p!, "Редактировать книгу")) return;

            if (index != -1 && p.IsReading)
                _listBooksOnReading[index] = p!;
            CollectionViewSource.GetDefaultView(ListBooks).Refresh();
            _userDialog.InformationMessage("Книга отредактирована");
        }

        #endregion

        #region DeleteBookCommand - Команда - удалить книгу

        ///<summary>Команда - удалить книгу</summary>
        private ICommand? _deleteBookCommand;

        ///<summary>Команда - удалить книгу</summary>
        public ICommand DeleteBookCommand => _deleteBookCommand
            ??= new LambdaCommand<MyBook?>(OnDeleteBookCommandExecuted, CanDeleteBookCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить книгу</summary>
        private bool CanDeleteBookCommandExecute(MyBook? p) => p is not null;

        ///<summary>Логика выполнения - удалить книгу</summary>
        private void OnDeleteBookCommandExecuted(MyBook? p)
        {
            ListBooksOnReading.Remove(p!);
            ListBooks.Remove(p!);
            OnPropertyChanged(nameof(ListBooks));
        }

        #endregion

        #region SaveBooksCommand - Команда - сохранить книги

        ///<summary>Команда - сохранить книги</summary>
        private ICommand? _saveBooksCommand;

        ///<summary>Команда - сохранить книги</summary>
        public ICommand SaveBooksCommand => _saveBooksCommand
            ??= new LambdaCommandAsync<IList<MyBook>>(OnSaveBooksCommandExecuted);

        ///<summary>Логика выполнения - сохранить книги</summary>
        private async Task OnSaveBooksCommandExecuted(IList<MyBook> p)
        {
            if (!await _workWithJSON.WriteFileAsync(@"Data/Books.json", p)) return;
            
            ((Command)SaveBooksCommand).Executable = false;
            _userDialog.InformationMessage("Список книг успешно сохранен");
        }

        #endregion

        #endregion

        private void ListBooks_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (e.OldIndex == -1) return;

            var book = _listBooks[e.OldIndex];
            if (book.IsReading ^ _listBooksOnReading.Contains(book))
            {
                if (book.IsReading)
                    ListBooksOnReading.Add(book);
                else ListBooksOnReading.Remove(book);
            }
            ((Command)SaveBooksCommand).Executable = true;
        }

    }
}
