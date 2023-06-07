namespace NBD.Models;

public class ComputerListModel
{
    public IEnumerable<ComputerModel> Computers { get; set; }
    public ComputerFilterModel Filter { get; set; }
}