namespace SmartVault.Program.BusinessObjects
{
    public partial class User : BaseBusinessObject
    {
        public string FullName => $"{FirstName} {LastName}";
    }
}
