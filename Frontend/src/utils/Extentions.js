export default function applyExtentions() {
    Array.prototype.firstOrDefault = function (predicate) {
        predicate ??= e => true;

        for (var i in this) {
            if (predicate(this[i])) {
                return this[i];
            }
        }
    }

    String.prototype.toPascalCase = function () {
        if (this.length === 1) {
            return this.toUpperCase();
        }

        if (this.length > 1) {
            return `${this.substring(0, 1).toUpperCase()}${this.substring(1)}`;
        }

        return this;
    }

    String.prototype.trimAll = function () {
        if (!this.length) {
            return this;
        }

        var value = this.replace(/\s+/g, ' ');

        return value.trim();
    }
}

applyExtentions();