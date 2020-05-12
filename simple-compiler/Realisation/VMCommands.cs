namespace simple_compiler.Classes
{
    public static class VMCommands
    {
        public static int cmStop = -1;
        public static int cmAdd = -2;
        public static int cmSub = -3;
        public static int cmMult = -4;
        public static int cmDiv = -5;
        public static int cmMod = -6;
        public static int cmNeg = -7;
        public static int cmLoad = -8;
        public static int cmSave = -9;
        public static int cmDup = -10;
        public static int cmDrop = -11;
        public static int cmSwap = -12;
        public static int cmOver = -13;
        public static int cmGOTO = -14;
        public static int cmIfEQ = -15;
        public static int cmIfNE = -16;
        public static int cmIfLE = -17;
        public static int cmIfLT = -18;
        public static int cmIfGE = -19;
        public static int cmIfGT = -20;
        public static int cmIn = -21;
        public static int cmOut = -22;
        public static int cmOutLn = -23;
    }
}