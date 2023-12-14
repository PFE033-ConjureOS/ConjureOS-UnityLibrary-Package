namespace ConjureOS.Input
{
    public static class ConjureArcadeControllerInfo
    {
        // The information in this class will need to change if we change the way the controller is made.
        // Everything in this class needs to be static since it is used at compile time to setup the Conjure Arcade Controller state description.

        public const string Interface = "HID";
        public const string Product = "Generic   USB  Joystick  "; // The extra spaces are normal as they are part of the product name created by the board's vendor

        public const int StateSizeInBytes = 8;
        public const int ReportIdByte = 0;
        public const int StickXByte = 1;
        public const int StickYByte = 1;
        public const int ButtonByte = 7;
        
        public enum ButtonBit : uint
        {
            Home    = 0,
            Start   = 1,
            
            One     = 2,
            Two     = 3,
            Three   = 4,
            
            A       = 5,
            B       = 6,
            C       = 7,
        }
    }
}