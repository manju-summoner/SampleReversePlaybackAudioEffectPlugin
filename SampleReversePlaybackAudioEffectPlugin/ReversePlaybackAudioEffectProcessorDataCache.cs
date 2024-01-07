using YukkuriMovieMaker.Player.Audio.Effects;

namespace SampleReversePlaybackAudioEffectPlugin
{
    internal class ReversePlaybackAudioEffectProcessorDataCache
    {
        public static ReversePlaybackAudioEffectProcessorDataCache Instance { get; } = new ();

        readonly Dictionary<ReversePlaybackAudioEffect, float[]> cache = [];
        readonly object lockObject = new ();

        public float[] GetData(ReversePlaybackAudioEffect effect, IAudioStream stream)
        {
            //マルチスレッドでアクセスする可能性があるため、Dictionaryの操作は排他制御する
            lock (lockObject)
            {
                if (!cache.TryGetValue(effect, out var data))
                {
                    data = new float[stream.Duration];
                    stream.Seek(0);//streamは事前にseekしておく必要がある
                    stream.Read(data, 0, data.Length);

                    //変換に時間が掛かる処理の場合はここで変換して、結果をキャッシュしておくと良い

                    cache.Add(effect, data);
                }

                //TODO: 一度作成したキャッシュは二度と破棄されないため、エフェクト側にキャッシュの削除ボタンを作ったり、一定数以上のキャッシュは古いものから破棄する処理を実装する必要がある。
                //これを実装しない場合はメモリリークする。

                return data;
            }

        }
    }
}