namespace EnforcingArchitecture.API.Common;

public class BaseResult
{
    public List<string> Errors { get; set; }
    public object Response { get; set; }

    public BaseResult()
    {
        Errors = new List<string>();
        Response = 0;
    }

    public void AddErrors(IList<string> erros)
    {
        Errors.AddRange(erros);
    }

    public bool IsValid()
    {
        return !Errors.Any();
    }

    public string GetErrorsMessages()
    {
        return string.Concat(Errors.ToArray());
    }
}
