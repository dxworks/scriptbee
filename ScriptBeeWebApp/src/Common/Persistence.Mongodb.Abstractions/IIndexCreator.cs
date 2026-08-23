namespace ScriptBee.Persistence.Mongodb;

public interface IIndexCreator
{
    Task Create(CancellationToken cancellationToken);
}
