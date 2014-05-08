using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mimry.Models
{
    public interface IDateCreated
    {
        DateTime CreatedDate { get; set; }
    }
    public interface IDateModified
    {
        DateTime LastModifiedDate { get; set; }
    }
    public interface IUserAction : IDateCreated, IDateModified
    {
        string User { get; set; }
    }
}