﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using ReactiveUI;

using OpenCRM.DataBase;
using OpenCRM.Controllers.Session;
using System.Data.SqlClient;
using System.Windows;

namespace OpenCRM.Models.Settings
{
    public class SettingsModel
    {
        #region "Values"
        int _userId;
        List<AccessRights> _userRightsAccess;
        
        #endregion

        #region "Properties"

        public List<Profile> Profiles { get; set; }
        
        #endregion

        #region "Constructor"
        public SettingsModel()
        {

        }

        public SettingsModel(int UserId, List<AccessRights> RightsAccess)
        {
            this._userId = UserId;
            this._userRightsAccess = RightsAccess;
            this.Profiles = getAllProfiles();
        }

        #endregion

        #region "Methods"

        #region "Edit User Data"
        private List<Profile> getAllProfiles()
        {
            var data = new List<Profile>();
            try
            {
                using (var _db = new OpenCRMEntities())
                {
                    var _profiles = (
                       from profile in _db.Profile
                       select profile
                    ).ToList();

                    data = _profiles;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return data;
        }

        public Profile getUserProfile()
        {
            var userProfile = Profiles.SingleOrDefault(x => x.ProfileId == this._userId);
            return userProfile;
        }

        public UserData getUserData()
        {
            UserData userData = null;

            try
            {
                using (var _db = new OpenCRMEntities())
                {
                    var user = _db.User.Single(x => x.UserId == this._userId);

                    userData = new UserData(
                        user.UserName,
                        user.Name,
                        user.LastName,
                        user.BirthDate.Value,
                        user.Email
                    );
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return userData;
        }

        public void SaveEditUser(UserData User)
        {
            try
            {
                using (var _db = new OpenCRMEntities())
                {
                    var user = _db.User.SingleOrDefault(x => x.UserId == this._userId);

                    user.BirthDate = Convert.ToDateTime(User.BirthDate);
                    user.Email = User.Email;
                    user.HashPassword = User.Password.GetHashCode().ToString();
                    user.LastName = User.LastName;
                    user.Name = User.Name;

                    _db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region "Create New User"

        public bool CheckUsername(string username)
        {
            try
            {
                using (var _db = new OpenCRMEntities())
                {
                    var userName = (
                       from user in _db.User
                       where user.UserName == username
                       select user
                    );
                    if (!userName.Any() && Validate("^[a-zA-Z][a-zA-Z0-9]{5,11}$", username))
                    {
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }

        public bool Validate(string pattern, string st)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(st, pattern);
        }

        public void SaveNewUser(string userName, string birthDate, string email, string password, string confirmPassword, string name, string lastName, ComboBox profile, Image imageEmail, Image imgProfile, Image imgPassword, Image imgConfirm, Button search)
        {
            #region Validate
            bool complete = true;
            var imgSearch = (search.Content as StackPanel).Children[0] as Image;
            if (Validate("^[a-zA-Z][a-zA-Z0-9]{5,11}$", userName) && CheckUsername(userName))
            {
                imgSearch.Source = new BitmapImage(new Uri("/Assets/Img/Correct.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgSearch.Source = new BitmapImage(new Uri("/Assets/Img/Wrong.png", UriKind.RelativeOrAbsolute));
                complete = false;
            }

            if (Validate("(?=.{8,})[a-zA-Z]+[^a-zA-Z]+|[^a-zA-Z]+[a-zA-Z]+", password))
            {
                imgPassword.Source = new BitmapImage(new Uri("/Assets/Img/Correct.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgPassword.Source = new BitmapImage(new Uri("/Assets/Img/Wrong.png", UriKind.RelativeOrAbsolute));
                complete = false;
            }
            if (password == confirmPassword && Validate("(?=.{8,})[a-zA-Z]+[^a-zA-Z]+|[^a-zA-Z]+[a-zA-Z]+", confirmPassword))
            {
                imgConfirm.Source = new BitmapImage(new Uri("/Assets/Img/Correct.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgConfirm.Source = new BitmapImage(new Uri("/Assets/Img/Wrong.png", UriKind.RelativeOrAbsolute));
                complete = false;
            }

            if (!Validate(
                    @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" +
                    @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", email))
            {
                imageEmail.Visibility = Visibility.Visible;
            }
            else
            {
                imageEmail.Visibility = Visibility.Hidden;
            }

            imgPassword.Visibility = Visibility.Visible;
            imgConfirm.Visibility = Visibility.Visible;

            if (profile.SelectedValue == null)
            {
                imgProfile.Visibility = Visibility.Visible;
                complete = false;
            }
            #endregion Validate

            if (complete)
            {
                try
                {
                    using (var _db = new OpenCRMEntities())
                    {
                        User user = _db.User.Create();
                        user.UserId = _db.User.Count() + 1;
                        user.UserName = userName;
                        user.Name = name;
                        user.BirthDate = Convert.ToDateTime(birthDate);
                        user.LastName = lastName;
                        user.HashPassword = password.GetHashCode().ToString();
                        user.Profile = _db.Profile.Find(profile.SelectedIndex);
                        user.Email = email;
                        user.CreateDate = DateTime.Now;
                        user.UpdateDate = DateTime.Now;
                        _db.User.Add(user);
                        _db.SaveChanges();
                        MessageBox.Show("User created.");
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        public void ValidateFields(string us)
        {

        }

        #endregion

        #region "Edit Permission"
        private List<AccessRights> getProfileAccessRight(int ProfileId)
        {
            var data = new List<AccessRights>();

            try
            {
                using (var db = new OpenCRMEntities())
                {
                    var query = (
                    from objects in db.Objects
                    from fields in db.Object_Fields
                    from profile in db.Profile
                    from profileObjects in db.Profile_Object
                    from profileObjectsFields in db.Profile_Object_Fields
                    where
                        ProfileId == profile.ProfileId &&
                        profile.ProfileId == profileObjects.ProfileId &&
                        profileObjects.ObjectId == objects.ObjectId &&
                        profileObjectsFields.ProfileObjectId == profileObjects.ProfileObjectId &&
                        profileObjectsFields.ObjectFieldsId == fields.ObjectFieldsId &&
                        objects.ObjectId == fields.ObjectId
                    select
                        new AccessRights()
                        {
                            ObjectId = objects.ObjectId,
                            ObjectName = objects.Name,
                            ObjectFielId = fields.ObjectFieldsId,
                            ObjectFieldName = fields.Name,
                            Read = profileObjectsFields.Read.Value,
                            Create = profileObjectsFields.Create.Value,
                            Modify = profileObjectsFields.Modify.Value
                        }
                    );

                    data = query.ToList();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return data;
        }

        public void LoadTabControl(int ProfileId, TabControl PermissionTabs)
        {
            PermissionTabs.Items.Clear();

            var profileAccessRights = getProfileAccessRight(ProfileId);

            if (!profileAccessRights.Any())
            {
                return;
            }

            var objetos = (
                from x in profileAccessRights
                group x by new { x.ObjectName } into temp
                select new { temp.Key }
            ).ToList();

            foreach (var item in objetos)
            {
                var tapItem = new TabItem();
                tapItem.Header = item.Key.ObjectName;
                tapItem.FontSize = 17;
                tapItem.Height = 50;
                tapItem.Visibility = Visibility.Visible;

                var grid = new Grid();
                grid.Background = new SolidColorBrush(Colors.DeepSkyBlue);
                tapItem.Content = grid;

                PermissionTabs.Items.Add(tapItem);
            }

            LoadSelectedProfile(ProfileId, PermissionTabs, profileAccessRights);
        }

        private void LoadSelectedProfile(int ProfileId, TabControl PermissionTabs, List<AccessRights> ProfileAccessRights)
        {
            var permissions = new Dictionary<string, List<AccessRights>>();

            var objectName = (
                from ob in ProfileAccessRights
                group ob by new { ob.ObjectName } into objts
                select new { objts.Key }
            ).ToList();

            objectName.ForEach(
                x => permissions.Add(x.Key.ObjectName, null)
            );

            var list = new List<AccessRights>();
            string actualName = ProfileAccessRights.First().ObjectName;

            foreach (var rightsAccess in ProfileAccessRights)
            {
                if (rightsAccess.ObjectName == actualName)
                {
                    list.Add(rightsAccess);
                }
                else
                {
                    permissions[actualName] = new List<AccessRights>(list);
                    actualName = rightsAccess.ObjectName;
                    list.Clear();
                }
            }

            permissions[actualName] = new List<AccessRights>(list);

            LoadTabControlGridData(permissions, PermissionTabs);
        }

        private void LoadTabControlGridData(Dictionary<string, List<AccessRights>> Permission, TabControl PermissionTab)
        {
            var ObjectsFieldsNames = new Dictionary<string, List<Label>>();
            var ObjectsFieldsCheckBox = new Dictionary<string, List<List<CheckBox>>>();

            foreach (var key in Permission)
            {
                var labelsObjectsFields = new List<Label>();
                var checkBoxesPermission = new List<List<CheckBox>>();

                foreach (var value in key.Value)
                {
                    var label = new Label();
                    label.Content = value.ObjectFieldName;
                    label.FontSize = 12;
                    labelsObjectsFields.Add(label);

                    var checkBoxes = new List<CheckBox>(3);

                    var checkBoxCreate = new CheckBox();
                    var checkBoxModify = new CheckBox();
                    var checkBoxRead = new CheckBox();

                    checkBoxCreate.IsChecked = value.Create;
                    checkBoxModify.IsChecked = value.Modify;
                    checkBoxRead.IsChecked = value.Read;

                    checkBoxes.Add(checkBoxCreate);
                    checkBoxes.Add(checkBoxRead);
                    checkBoxes.Add(checkBoxModify);

                    checkBoxesPermission.Add(checkBoxes);
                }

                ObjectsFieldsNames.Add(key.Key, labelsObjectsFields);
                ObjectsFieldsCheckBox.Add(key.Key, checkBoxesPermission);
            }

            foreach (TabItem itemTab in PermissionTab.Items)
            {
                var itemGrid = itemTab.Content as Grid;

                //Create Columns
                for (int i = 0; i < 4; i++)
                {
                    var columnDefinition = new ColumnDefinition();

                    if (i == 0)
                        columnDefinition.Width = new GridLength(70, GridUnitType.Star);
                    else
                        columnDefinition.Width = new GridLength(10, GridUnitType.Star);

                    itemGrid.ColumnDefinitions.Add(columnDefinition);
                }

                var count = ObjectsFieldsNames[itemTab.Header.ToString()].Count;

                //Create Rows
                for (int i = 0; i < count; i++)
                {
                    var rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(20, GridUnitType.Auto);

                    itemGrid.RowDefinitions.Add(rowDefinition);
                }

                //Add labels and checkbox to the Grid
                for (int i = 0; i < count; i++)
                {
                    var itemHeader = itemTab.Header.ToString();
                    Grid.SetRow(ObjectsFieldsNames[itemHeader][i], i);
                    Grid.SetRow(ObjectsFieldsCheckBox[itemHeader][i][0], i);
                    Grid.SetRow(ObjectsFieldsCheckBox[itemHeader][i][1], i);
                    Grid.SetRow(ObjectsFieldsCheckBox[itemHeader][i][2], i);

                    Grid.SetColumn(ObjectsFieldsNames[itemHeader][i], 0);
                    Grid.SetColumn(ObjectsFieldsCheckBox[itemHeader][i][0], 1);
                    Grid.SetColumn(ObjectsFieldsCheckBox[itemHeader][i][1], 2);
                    Grid.SetColumn(ObjectsFieldsCheckBox[itemHeader][i][2], 3);

                    itemGrid.Children.Add(ObjectsFieldsNames[itemHeader][i]);
                    itemGrid.Children.Add(ObjectsFieldsCheckBox[itemHeader][i][0]);
                    itemGrid.Children.Add(ObjectsFieldsCheckBox[itemHeader][i][1]);
                    itemGrid.Children.Add(ObjectsFieldsCheckBox[itemHeader][i][2]);
                }
            }
        }

        public void SavePermission(TabControl PermissionTab, int SelectedProfileId)
        {
            var newListAccessRights = new List<AccessRights>();

            foreach (TabItem itemTab in PermissionTab.Items)
            {
                var itemGrid = itemTab.Content as Grid;
                var itemHeader = itemTab.Header.ToString();

                for (int i = 0; i < itemGrid.RowDefinitions.Count; i++)
                {
                    var newAccessRight = new AccessRights();
                    newAccessRight.ObjectName = itemHeader;

                    for (int j = 0; j < itemGrid.ColumnDefinitions.Count; j++)
                    {
                        var itemSelected = itemGrid.Children.Cast<UIElement>().First(
                           x => Grid.GetRow(x) == i && Grid.GetColumn(x) == j
                        );

                        if (j == 0)
                        {
                            var item = itemSelected as Label;
                            newAccessRight.ObjectFieldName = item.Content.ToString();
                        }
                        else
                        {
                            var item = itemSelected as CheckBox;

                            if (j == 1)
                                newAccessRight.Read = item.IsChecked.Value;
                            else if (j == 2)
                                newAccessRight.Create = item.IsChecked.Value;
                            else
                                newAccessRight.Modify = item.IsChecked.Value;
                        }
                    }
                    newListAccessRights.Add(newAccessRight);
                }
            }

            try
            {
                using (var db = new OpenCRMEntities())
                {
                    var query = (
                        from objects in db.Objects
                        from fields in db.Object_Fields
                        from profile in db.Profile
                        from profileObjects in db.Profile_Object
                        from profileObjectsFields in db.Profile_Object_Fields
                        where
                            SelectedProfileId == profile.ProfileId &&
                            profile.ProfileId == profileObjects.ProfileId &&
                            profileObjects.ObjectId == objects.ObjectId &&
                            profileObjectsFields.ProfileObjectId == profileObjects.ProfileObjectId &&
                            profileObjectsFields.ObjectFieldsId == fields.ObjectFieldsId &&
                            objects.ObjectId == fields.ObjectId
                        select
                            profileObjectsFields
                    );

                    foreach (var item in newListAccessRights)
                    {
                        Profile_Object_Fields selectedRow = query.SingleOrDefault(
                            x =>
                                x.Object_Fields.Name == item.ObjectFieldName &&
                                x.Object_Fields.Objects.Name == item.ObjectName
                        );

                        if (!(selectedRow.Modify == item.Modify && selectedRow.Create == item.Create && selectedRow.Read == item.Read))
                        {
                            selectedRow.Modify = item.Modify;
                            selectedRow.Create = item.Create;
                            selectedRow.Read = item.Read;

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion
        
        #endregion
    }

    public class UserData
    {
        #region "Properties"
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        #endregion

        #region "Constructors"
        public UserData(string Name, string Lastname, DateTime BirthDate, string Email)
        {
            this.Name = Name;
            this.LastName = Lastname;
            this.BirthDate = BirthDate.ToShortDateString();
            this.Email = Email;
        }

        public UserData(string Username, string Name, string Lastname, DateTime BirthDate, string Email)
            :this(Name, Lastname, BirthDate, Email)
        {
            this.UserName = Username;
        }

        #endregion

    }

}
