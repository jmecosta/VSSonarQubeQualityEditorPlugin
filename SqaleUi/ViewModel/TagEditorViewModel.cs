﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tageditorviewmodel.cs" company="Copyright © 2014 jmecsoftware">
//     Copyright (C) 2014 [jmecsoftware, jmecsoftware2014@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using ExtensionTypes;

    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    using SonarRestService;

    using SqaleUi.Menus;

    /// <summary>
    ///     The tag editor view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class TagEditorViewModel
    {
        #region Fields

        /// <summary>
        ///     The conf.
        /// </summary>
        private readonly ISonarConfiguration conf;

        /// <summary>
        ///     The model.
        /// </summary>
        private readonly ISqaleGridVm model;

        /// <summary>
        ///     The service.
        /// </summary>
        private readonly ISonarRestService service;

        /// <summary>
        /// The selected tag in rule.
        /// </summary>
        private string selectedTagInRule;

        /// <summary>
        /// The selected tag in server.
        /// </summary>
        private string selectedTagInServer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TagEditorViewModel" /> class.
        /// </summary>
        public TagEditorViewModel()
        {
            this.SelectedTags = new List<string>();
            this.AvailableTags = new ObservableCollection<string>();
            this.TagsInRule = new ObservableCollection<string>();
            this.CanExecuteAddSelectedTags = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagEditorViewModel"/> class.
        /// </summary>
        /// <param name="conf">
        /// The conf.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        public TagEditorViewModel(ISonarConfiguration conf, ISonarRestService service, ISqaleGridVm model)
        {
            this.conf = conf;
            this.service = service;
            this.model = model;

            this.SelectedTags = new List<string>();
            this.AvailableTags = new ObservableCollection<string>();
            this.TagsInRule = new ObservableCollection<string>();

            this.CanExecuteAddSelectedTags = false;
            this.CanExecuteRefreshTags = false;
            this.CanExecuteRemoveSelected = false;
            this.AddSelectedTagCommand = new RelayCommand(this.ExecuteAddSelectedTags);
            this.RefreshTagsCommand = new RelayCommand(this.RefreshAvailableTagsInServer, () => this.CanExecuteRefreshTags);
            this.RemoveSelectedCommand = new RelayCommand(this.ExecuteRemoveSelected);

            this.SelectionChangedCommand = new RelayCommand<List<string>>(items => { this.SelectedTags = items; });

            this.RefreshAvailableTagsInServer();

            this.RefreshTagsInRule();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the add selected tags command.
        /// </summary>
        public ICommand AddSelectedTagCommand { get; set; }

        /// <summary>
        ///     The available tags.
        /// </summary>
        public ObservableCollection<string> AvailableTags { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether can execute add selected tags.
        /// </summary>
        public bool CanExecuteAddSelectedTags { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether can execute refresh tags.
        /// </summary>
        public bool CanExecuteRefreshTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can execute remove selected.
        /// </summary>
        public bool CanExecuteRemoveSelected { get; set; }

        /// <summary>
        ///     Gets or sets the refresh tags command.
        /// </summary>
        public ICommand RefreshTagsCommand { get; set; }

        /// <summary>
        /// Gets or sets the remove selected command.
        /// </summary>
        public RelayCommand RemoveSelectedCommand { get; set; }

        /// <summary>
        /// Gets or sets the selected tag in rule.
        /// </summary>
        public string SelectedTagInRule
        {
            get
            {
                return this.selectedTagInRule;
            }

            set
            {
                this.selectedTagInRule = value;
                if (value == null)
                {
                    this.CanExecuteRemoveSelected = false;
                }
                else
                {
                    this.CanExecuteRemoveSelected = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected tag in server.
        /// </summary>
        public string SelectedTagInServer
        {
            get
            {
                return this.selectedTagInServer;
            }

            set
            {
                this.selectedTagInServer = value;
                if (value == null)
                {
                    this.CanExecuteAddSelectedTags = false;
                }
                else
                {
                    this.CanExecuteAddSelectedTags = true;
                }
            }
        }

        /// <summary>
        ///     The selected tags.
        /// </summary>
        public List<string> SelectedTags { get; set; }

        /// <summary>
        ///     Gets the selection changed command.
        /// </summary>
        public RelayCommand<List<string>> SelectionChangedCommand { get; private set; }

        /// <summary>
        /// Gets or sets the tags in rule.
        /// </summary>
        public ObservableCollection<string> TagsInRule { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The refresh available tags in server.
        /// </summary>
        public void RefreshAvailableTagsInServer()
        {
            List<string> tags = this.service.GetAllTags(this.conf);
            this.AvailableTags.Clear();
            foreach (string tag in tags)
            {
                this.AvailableTags.Add(tag);
            }
        }

        /// <summary>
        /// The refresh tags in rule.
        /// </summary>
        public void RefreshTagsInRule()
        {
            if (this.model.SelectedRule == null)
            {
                return;
            }

            this.TagsInRule.Clear();
            foreach (string tag in this.model.SelectedRule.Tags)
            {
                this.TagsInRule.Add(tag);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The aggregate list.
        /// </summary>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string AggregateList(string arg1, string arg2)
        {
            return arg1 + "\r\n" + arg2;
        }

        /// <summary>
        ///     The execute add selected tags.
        /// </summary>
        private void ExecuteAddSelectedTags()
        {
            if (this.SelectedTagInServer == null)
            {
                return;
            }

            var newList = new List<string>();

            foreach (string tag in this.TagsInRule)
            {
                if (tag.EndsWith(this.SelectedTagInServer))
                {
                    return;
                }

                newList.Add(tag);
            }

            newList.Add(this.SelectedTagInServer);

            this.SetTagsInRule(newList);
            this.RefreshTagsInRule();
        }

        /// <summary>
        /// The execute remove selected.
        /// </summary>
        private void ExecuteRemoveSelected()
        {
            if (this.SelectedTagInRule == null)
            {
                return;
            }

            var newList = new List<string>();

            foreach (string tag in this.TagsInRule)
            {
                if (tag == null || tag.Equals(this.SelectedTagInRule))
                {
                    continue;
                }

                newList.Add(tag);
            }

            this.SetTagsInRule(newList);

            this.model.SelectedRule.Tags.Remove(this.SelectedTagInRule);
            this.RefreshTagsInRule();
        }

        /// <summary>
        /// The set tags in rule.
        /// </summary>
        /// <param name="newList">
        /// The new list.
        /// </param>
        private void SetTagsInRule(List<string> newList)
        {
            List<string> errorMessage = this.service.UpdateTags(this.conf, this.model.SelectedRule, newList);

            if (errorMessage.Count == 0)
            {
                this.model.SelectedRule.Tags.Add(this.SelectedTagInServer);
            }
            else
            {
                MessageBox.Show("Error: " + errorMessage.Aggregate(this.AggregateList));
            }
        }

        #endregion
    }
}