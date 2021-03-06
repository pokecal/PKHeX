using System.Linq;

namespace PKHeX.Core
{
    public class Mail2 : Mail
    {
        private readonly bool US;

        public Mail2(SAV2 sav, int index)
        {
            US = !sav.Japanese && !sav.Korean;
            DataOffset = index < 6 ? (index * 0x2F) + 0x600 : ((index - 6) * 0x2F) + 0x835;
            Data = sav.GetData(DataOffset, 0x2F);
        }

        public override string GetMessage(bool isLastLine) => US ? StringConverter12.GetString1(Data, isLastLine ? 0x11 : 0, 0x10, false) : string.Empty;

        public override void SetMessage(string line1, string line2)
        {
            if (US)
            {
                StringConverter12.SetString1(line2, 0x10, false, 0x10, 0x50).CopyTo(Data, 0x11);
                StringConverter12.SetString1(line1, 0x10, false, 0x10, (ushort)(Data.Skip(0x11).Take(0x10).All(v => v == 0x50) ? 0x50 : 0x7F)).CopyTo(Data, 0);
                Data[0x10] = 0x4E;
            }
        }

        public override string AuthorName
        {
            get => US ? StringConverter12.GetString1(Data, 0x21, 7, false) : string.Empty;
            set
            {
                if (US)
                {
                    StringConverter12.SetString1(value, 7, false, 8, 0x50).CopyTo(Data, 0x21);
                    Data[0x29] = Data[0x2A] = 0;
                }
            }
        }

        public override ushort AuthorTID
        {
            get => (ushort)(Data[0x2B] << 8 | Data[0x2C]);
            set
            {
                Data[0x2B] = (byte)(value >> 8);
                Data[0x2C] = (byte)(value & 0xFF);
            }
        }

        public override int AppearPKM { get => Data[0x2D]; set => Data[0x2D] = (byte)value; }
        public override int MailType { get => Data[0x2E]; set => Data[0x2E] = (byte)value; }

        public override bool? IsEmpty
        {
            get
            {
                if (MailType == 0) return true;
                else if (MailType == 0x9E || (MailType >= 0xB5 && MailType <= 0xBD)) return false;
                else return null;
            }
        }

        public override void SetBlank() => (new byte[0x2F]).CopyTo(Data, 0);
    }
}