using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;

namespace Mimry.ViewModels
{
    public class MimView
    {
        public Guid MimID { get; set; }
        public int Vote { get; set; }
    }
}