import Enumerable from "linq";

export default class Common {
    static transformErrorData(error) {
        if (error?.response?.data?.errors) {
            if (error.response.data.errors.toString() === error.response.data.errors) {
                return error.response.data.errors;
            } else {
                return Enumerable
                    .from(error.response.data.errors)
                    .select(e => <div>{e.key}: {e.value?.join(',')}</div>)
                    .toArray();
            }
        }
        else {
            return error?.response?.data ? JSON.stringify(error?.response?.data) : 'N/A';
        }
    }
}