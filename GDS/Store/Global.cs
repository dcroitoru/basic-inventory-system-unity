namespace GDS {
    public static class Global {
        public static readonly EventBus GlobalBus = new();
        public static readonly EventBus BasicBus = new();

        public static BasicInventoryStore BasicInventoryStore = new();
    }
}