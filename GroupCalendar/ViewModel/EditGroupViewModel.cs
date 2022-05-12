using Firebase.Storage;
using GroupCalendar.Core;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.View;
using GroupCalendar.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace GroupCalendar.ViewModel
{
    public class EditGroupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SendCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand SearchImageCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand MakeAdminCommand { get; set; }
        public ICommand RemoveAdminCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand ShowInviteLinkCommand { get; private set; }

        public GroupModel Group { get; set; } = new GroupModel();
        public Uri Image { get; set; } = new Uri("https://uning.es/wp-content/uploads/2016/08/ef3-placeholder-image.jpg");
        public bool IsEditing { get; private set; }
        public Guid GroupId { get; set; }

        public bool IsLoading { get; private set; }
        public bool ImageChanged { get; private set; } = false;
        public ObservableCollection<UserModel> Users { get; private set; }

        bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());


        public EditGroupViewModel()
        {
            IsEditing = ApplicationState.GetValue<bool>("is_editing");
            if (IsInDesignMode)
            {
                IsEditing = true;
            }
            if (IsEditing)
            {
                GroupId = ApplicationState.GetValue<Guid>("group_id");
                LoadGroup();
            }
            SendCommand = new RelayCommand(o => SendGroup(), o => CanSend());
            BackCommand = new RelayCommand(o => Back());
            SearchImageCommand = new RelayCommand(o => SearchImage());
            DeleteCommand = new RelayCommand(o => Delete());
            MakeAdminCommand = new RelayCommand(o => MakeAdmin(o));
            RemoveAdminCommand = new RelayCommand(o => RemoveAdmin(o));
            RemoveUserCommand = new RelayCommand(o => RemoveUser(o));
            ShowInviteLinkCommand = new RelayCommand(o => ShowInviteLink());
        }

        private async void MakeAdmin(object o)
        {
            var userId = (string)o;
            if (!Group.Admins.Contains(userId))
            {
                Group.Admins.Add(userId);
            }
            OnPropertyChanged(nameof(Group));
            await UpdateGroupRepositoryAsync();
            UpdateUserRole();
        }

        private async void RemoveAdmin(object o)
        {
            var userId = (string)o;
            Group.Admins.Remove(userId);
            OnPropertyChanged(nameof(Group));
            await UpdateGroupRepositoryAsync();
            UpdateUserRole();
        }

        private async void RemoveUser(object o)
        {
            var userId = (string)o;
            Group.Admins.Remove(userId);
            Group.Users.Remove(userId);
            await UpdateGroupRepositoryAsync();

            var userToRemove = new List<UserModel>(Users).Find(user => user.Id == userId);
            if (userToRemove != null)
            {
                userToRemove.Groups.Remove(GroupId);
                await Repository.UpdateUserAsync(userToRemove);

                await LoadUsers();

                var uid = ApplicationState.GetValue<string>("uid");
                // if the user removes himself from the group, he will go back to timetablepage, on anothers group page
                if (userToRemove.Id == uid)
                {
                    ApplicationState.SetValue("group_id", userToRemove.Groups[0]);
                    StaticResources.mainWindow.Frame.Content = new TimetablePage();
                }
            }
            OnPropertyChanged(nameof(Users));
        }

        private async Task UpdateGroupRepositoryAsync()
        {
            await Repository.UpdateGroupAsync(Group);
            OnPropertyChanged(nameof(Group));
        }

        private async void Delete()
        {
            await LoadUsers();
            var updateTasks = new List<UserModel>(Users).ConvertAll(async (user) =>
            {
                user.Groups.Remove(Group.Id);
                return await Repository.UpdateUserAsync(user);
            });
            await Task.WhenAll(updateTasks);
            await Repository.DeleteGroupAsync(Group.Id);
            var uid = ApplicationState.GetValue<string>("uid");
            ApplicationState.SetValue("group_id", new List<UserModel>(Users).Find(user => user.Id == uid).Groups[0]);
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        private async Task LoadUsers()
        {
            var tasks = Group.Users.ConvertAll(user => Repository.GetUserByIdAsync(user));
            Users = new ObservableCollection<UserModel>(await Task.WhenAll(tasks));
            UpdateUserRole();
        }

        private void UpdateUserRole()
        {
            new List<UserModel>(Users).ForEach(user =>
            {
                if (Group.Admins.Contains(user.Id))
                {
                    user.Role = "Admin";
                }
                else
                {
                    user.Role = "Usuario";
                }
            });
            OnPropertyChanged(nameof(Users));
        }

        private void SearchImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Group.Image = new Uri(openFileDialog.FileName);
                File.Copy(Group.Image.OriginalString, ".\\tempImg", true);
                Image = new Uri(Environment.CurrentDirectory + "\\tempImg");
                OnPropertyChanged("Image");
                ImageChanged = true;
            }
        }

        private void Back()
        {
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        private bool CanSend()
        {
            return !IsLoading && Group.Name.Trim() != "";
        }

        private async void LoadGroup()
        {
            Group = await Repository.GetGroupByIdAsync(GroupId.ToString());
            OnPropertyChanged("Group");
            Image = Group.Image;
            OnPropertyChanged("Image");
            await LoadUsers();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShowInviteLink()
        {
            var link = $"https://groupcalendar.djessyczaplicki.com/invite/{GroupId}";
            var message = $"¡Se ha copiado la invitación '{link}' al portapapeles!";
            System.Windows.Clipboard.SetText(link);
            System.Windows.MessageBox.Show(message);
        }


        private async void SendGroup()
        {
            IsLoading = true;
            if (ImageChanged)
            {
                var token = ApplicationState.GetValue<string>("token");
                var stream = File.Open(Group.Image.OriginalString, FileMode.Open);
                var task = new FirebaseStorage(
                    "groupcalendar-53829.appspot.com",
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(token),
                        ThrowOnCancel = true
                    })
                    .Child("groups")
                    .Child(Guid.NewGuid().ToString())
                    .PutAsync(stream);
                Group.Image = new Uri(await task);
            }

            if (!IsEditing)
            {
                var uid = ApplicationState.GetValue<string>("uid");
                Group.Users.Add(uid);
                Group.Admins.Add(uid);
                var user = await Repository.GetUserByIdAsync(uid);
                user.Groups.Add(Group.Id);
                await Repository.UpdateUserAsync(user);
            }
            await Repository.UpdateGroupAsync(Group);
            IsLoading = false;
            Back();
        }


    }
}
