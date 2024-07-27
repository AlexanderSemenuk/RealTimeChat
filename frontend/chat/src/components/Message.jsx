const getSentimentColor = (sentiment) => {
    switch (sentiment.toLowerCase()) {
        case 'positive':
            return '#66bb6a'; 
        case 'negative':
            return '#ef5350';
        case 'neutral':
            return '#bdbdbd'; 
        case 'mixed':
            return '#ffee58'; 
        default:
            return 'inherit'; 
    }
};

export const Message = ({ messageInfo }) => {
    const sentimentColor = getSentimentColor(messageInfo.sentiment);

    return (
        <div className="w-fit">
            <span className="text-sm text-slate-600">{messageInfo.userName}</span>
            <div 
                className="p-2 rounded-lg shadow-md" 
                style={{ 
                    backgroundColor: `${sentimentColor}22`,
                    borderLeft: `4px solid ${sentimentColor}`
                }}
            >
                {messageInfo.message}
            </div>
            <span className="text-xs italic" style={{ color: sentimentColor }}>
                Sentiment: {messageInfo.sentiment}
            </span>
        </div>
    );
};