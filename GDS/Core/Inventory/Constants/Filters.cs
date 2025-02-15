namespace GDS.Core {

    public static class Filters {
        public static readonly FilterFn Everything = (_) => true;
        public static readonly FilterFn Nothing = (_) => false;
    }
}