export default function ApplyExtentions() {
    Array.prototype.firstOrDefault = function (predicate) {
        predicate ??= e => true;

        for (var i in this) {
            if (predicate(this[i])) {
                return this[i];
            }
        }
    }

    Array.prototype.any = function (predicate) {
        predicate ??= e => true;

        for (var i in this) {
            if (predicate(this[i])) {
                return this[i];
            }
        }
    }
}