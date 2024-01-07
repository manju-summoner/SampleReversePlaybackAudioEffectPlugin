//#define EnableGlobalCache
using YukkuriMovieMaker.Player.Audio.Effects;

namespace SampleReversePlaybackAudioEffectPlugin
{
    internal class ReversePlaybackAudioEffectProcessor(ReversePlaybackAudioEffect effect) : AudioEffectProcessorBase
    {
        public override int Hz => Input?.Hz ?? throw new NullReferenceException($"{nameof(Input)} is null");

        public override long Duration => Input?.Duration ?? throw new NullReferenceException($"{nameof(Input)} is null");

        long currentPosition;

#if !EnableGlobalCache
        float[]? cache;
#endif

        protected override int read(float[] destBuffer, int offset, int count)
        {
            if (Input is null)
                return 0;

#if EnableGlobalCache
            var cache = ReversePlaybackAudioEffectProcessorDataCache.Instance.GetData(effect, Input);
#else
            //プレビューの再生ごとにReversePlaybackAudioEffectProcessorが再生成されるため、キャッシュは1回のプレビュー内でしか使われない。
            //そのため、プレビューボタンを押すたびにこの処理が走る。

            //一度作成したキャッシュを使いまわしたい場合は、このファイルの1行目（//#define EnableGlobalCache）のコメントアウトを外してください。
            //実装が十分でないため、ReversePlaybackAudioEffectProcessorDataCache.csのコメントも参照してください。
            if (cache is null)
            {
                cache = new float[Input.Duration];
                Input.Seek(0);//streamは事前にseekしておく必要がある
                Input.Read(cache, 0, cache.Length);
            }
#endif
            var readCount = Math.Min(count, cache.Length);
            for (int i = 0; i < readCount / 2; i++)
            {
                var destBufferPosition = offset + i * 2;
                var cachePosition = cache.Length - 1 - currentPosition - (i + 1) * 2;
                destBuffer[destBufferPosition] = cache[cachePosition];
                destBuffer[destBufferPosition + 1] = cache[cachePosition + 1];
            }
            currentPosition += readCount;
            return readCount;
        }

        protected override void seek(long position)
        {
            currentPosition = position;
        }
    }
}