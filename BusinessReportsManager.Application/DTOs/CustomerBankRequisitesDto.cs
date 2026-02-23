using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessReportsManager.Application.DTOs;

public class CustomerBankRequisitesDto : CustomerBankRequisitesCreateDto
{
    public Guid Id { get; set; }
}
