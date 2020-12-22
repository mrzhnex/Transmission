using Android.Media;

namespace Mobile
{
    public class StreamMediaDataSource : MediaDataSource
    {
        private System.IO.Stream Data { get; set; }
        public StreamMediaDataSource(System.IO.Stream Data)
        {
            this.Data = Data;
        }
        public override long Size
        {
            get
            {
                return Data.Length;
            }
        }
        public override int ReadAt(long position, byte[] buffer, int offset, int size)
        {
            Data.Seek(position, System.IO.SeekOrigin.Begin);
            return Data.Read(buffer, offset, size);
        }
        public override void Close()
        {
            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }
    }
}