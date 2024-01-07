using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace SampleReversePlaybackAudioEffectPlugin
{
    [AudioEffect("逆再生", ["サンプルプラグイン"], [])]
    public class ReversePlaybackAudioEffect : AudioEffectBase
    {
        public override string Label => "逆再生";

        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new ReversePlaybackAudioEffectProcessor(this);
        }

        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        protected override IEnumerable<IAnimatable> GetAnimatables()
        {
            return [];
        }
    }
}
