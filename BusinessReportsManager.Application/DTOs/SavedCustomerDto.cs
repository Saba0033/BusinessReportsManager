using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessReportsManager.Application.DTOs;

public class SavedCustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}
