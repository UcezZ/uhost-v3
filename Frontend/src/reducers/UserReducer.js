const initialState = {
    roles: [],
    rights: [],
    email: 'ucezz@ucezz.sytes.net',
    name: 'Суперадмин',
    description: '',
    login: 'admin',
    theme: 'Dark'
}
export default function UserReducer(state = initialState, action) {
    switch (action.type) {
        case 'users/userLoggedIn': {
            return action.payload;
        }
        default: return state;
    }
}