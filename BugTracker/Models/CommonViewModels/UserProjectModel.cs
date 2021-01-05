using BugTracker.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.CommonViewModels
{
    public class UserProjectModel
    {
        public UserProjectModel(Project projectModel, UserIndexModel userModel)
        {
            this.ProjectModel = projectModel;
            this.UserModel = userModel;
        }

        public Project ProjectModel { get; set; }

        public UserIndexModel UserModel { get; set; }
    }
}
