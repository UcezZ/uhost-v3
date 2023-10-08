export default class Store {
  #subscribers;
  #reducers;
  #state;

  constructor(reducers = {}, initialState = {}) {
    this.subscribers = [];
    this.reducers = reducers;
    this.state = this.reduce(initialState, {});
  }

  get value() {
    return this.state;
  }

  /**
   * 
   * @param {function(*)} fn 
   * @returns 
   */
  subscribe(fn) {
    this.subscribers = [...this.subscribers, fn];
    fn(this.value);
    return () => {
      this.subscribers = this.subscribers.filter(sub => sub !== fn);
    };
  }

  /**
   * 
   * @param {*} action 
   */
  dispatch(action) {
    this.state = this.reduce(this.state, action);
    this.subscribers.forEach(fn => fn(this.value));
  }

  /**
   * 
   * @param {{ [x: string]: any; }} state 
   * @param {{}} action 
   * @returns 
   */
  #reduce(state, action) {
    const newState = {};
    for (const prop in this.reducers) {
      newState[prop] = this.reducers[prop](state[prop], action);
    }
    return newState;
  }
}