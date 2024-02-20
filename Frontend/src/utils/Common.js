export default class Common {
    static tokenKey = 'accessToken';

    static transformErrorData(error) {
        if (error?.response?.data?.errors) {
            if (error.response.data.errors instanceof String) {
                return error.response.data.errors;
            } else {
                return Object
                    .keys(error.response.data.errors)
                    .map(e => `${e}: ${error.response.data.errors[e]?.join(', ')}`);
            }
        }
        else {
            return error?.response?.data ? JSON.stringify(error?.response?.data, null, 2) : 'N/A';
        }
    }

    /**
     * Determines if access token is present
     * @returns {Boolean}
     */
    static isTokenPresent() {
        return !!localStorage.getItem(this.tokenKey) || !!sessionStorage.getItem(this.tokenKey);
    }

    static resetToken() {
        localStorage.removeItem(this.tokenKey);
        sessionStorage.removeItem(this.tokenKey);
    }
}