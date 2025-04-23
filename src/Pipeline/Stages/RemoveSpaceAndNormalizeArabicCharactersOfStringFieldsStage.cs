using System.Threading.Tasks;
using Quantum.Core;

namespace Quantum.Command.Pipeline.Stages;

public class RemoveSpaceAndNormalizeArabicCharactersOfStringFieldsStage : IAmAPipelineStage
{
    public override async Task Process<T>(T command, StageContext context)
    {
        T newCommand = Normalize(command);
    }

    private T Normalize<T>(T command) where T : IAmACommand
    {
        command.NormalizeArabicCharacters();

        return command;
    }
}