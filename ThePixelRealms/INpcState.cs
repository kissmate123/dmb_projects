using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ThePixelRealms
{
    public interface INpcState
    {
        List<Npc> npcs { get; }

        Npc currentNpc { get; set; }

        bool isGreetingActive { get; set; }
        double npcSpeechTimer { get; set; }

        TextBlock npcSpeech { get; set; }
        Image npcBubbleLeft { get; set; }
        Image npcBubbleMiddle { get; set; }
        Image npcBubbleRight { get; set; }

        TextBlock interactionHint { get; set; }

        Dictionary<int, NpcType> talkSequence { get; set; }
        HashSet<NpcType> greetingAllowedTypes { get; set; }
    }
}
