using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_interactions_framework
{
    public enum ExampleEnum
    {
        First,
        Second,
        Third,
        Fourth,
        [Display(Name = "Twenty First")]
        TwentyFirst
    }
}
