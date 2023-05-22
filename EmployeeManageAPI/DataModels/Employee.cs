namespace EmployeeManageAPI.DataModels
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public String EmployeeName { get; set; }
        public String EmployeeEmail { get; set; }
        public String EmployeePassword { get; set; }
        public Boolean IsAdmin { get; set; }
    }
}
