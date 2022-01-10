using System.Threading.Tasks;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.SyncDataServices.Http
{
    public interface IEnrollmentDataClient
    {
        Task CreateEnrollmentFromPaymentService(CreateEnrollmentDto enrollment);
    }
}