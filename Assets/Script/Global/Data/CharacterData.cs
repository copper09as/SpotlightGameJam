namespace Global.Data
{
    public class CharacterData
    {
        public float ScaleX;
        public float ScaleY;
        public float Hp;
        public float JumpHeight;
        public float Damage;
        public float speed;
        public float Gravity;
        public CharacterEntityData ToEntityData()
        {
            return new CharacterEntityData
            {
                ScaleX = this.ScaleX,
                ScaleY = this.ScaleY,
                Hp = this.Hp,
                JumpHeight = this.JumpHeight,
                Damage = this.Damage,
                speed = this.speed,
                Gravity = this.Gravity,
                signId = -1
            };
        }
    }
    public class CharacterEntityData
    {
        public float ScaleX;
        public float ScaleY;
        public float Hp;
        public float JumpHeight;
        public float Damage;
        public float speed;
        public float Gravity;
        public int signId = -1;
    }
}
