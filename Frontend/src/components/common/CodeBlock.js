import Styles from '../../ui/Styles';

export default function CodeBlock({ data, json, ...props }) {
    if (json) {
        data = JSON.stringify(data, null, 2);
    }

    return (
        <div style={{ ...Styles.code, ...props }}>
            {data}
        </div>
    );
}