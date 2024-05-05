export default function applyExtentions() {
    /**
     * 
     * @param {function} predicate 
     * @returns 
     */
    Array.prototype.firstOrDefault = function (predicate) {
        predicate ??= e => true;

        for (var i in this) {
            if (predicate(this[i])) {
                return this[i];
            }
        }
    }

    /**
     * 
     * @param {function} keySelector 
     * @param {function} valueSelector 
     * @returns {{}}
     */
    Array.prototype.toDictionary = function (keySelector, valueSelector) {
        keySelector ??= e => e?.toString() ?? 'n/a';
        valueSelector ??= e => e?.toString() ?? 'n/a';

        var dict = {};

        for (var i in this) {
            var key = keySelector(this[i]);
            var value = valueSelector(this[i]);

            dict[key] = value;
        }

        return dict;
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

    String.prototype.toKebabCase = function () {
        var value = this.replace(/([A-Z])/g, '.$1').toLowerCase();

        if (value.startsWith('.')) {
            return value.slice(1);
        }

        return value;
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