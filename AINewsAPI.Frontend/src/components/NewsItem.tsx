import React from 'react';

interface NewsItemProps {
  title: string;
  description: string;
  url: string;
  publishedAt: string;
}

const decodeHtml = (html: string) => {
  const txt = document.createElement('textarea');
  txt.innerHTML = html;
  return txt.value;
};

const formatDate = (dateString: string): string => {
  if (!dateString) return 'Date not available';
  
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return 'Invalid date';

  const now = new Date();
  const diffTime = Math.abs(now.getTime() - date.getTime());
  const diffHours = Math.floor(diffTime / (1000 * 60 * 60));
  const diffDays = Math.floor(diffHours / 24);

  const fullDate = date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });

  if (diffHours < 1) {
    return `Less than an hour ago (${fullDate})`;
  } else if (diffHours < 24) {
    return `${diffHours} hour${diffHours === 1 ? '' : 's'} ago (${fullDate})`;
  } else if (diffDays < 7) {
    return `${diffDays} day${diffDays === 1 ? '' : 's'} ago (${fullDate})`;
  } else {
    return fullDate;
  }
};

const NewsItem: React.FC<NewsItemProps> = ({ title, description, url, publishedAt }) => {
  return (
    <div className="news-item">
      <h2>{decodeHtml(title)}</h2>
      <p className="publication-date">
        Published: {formatDate(publishedAt)}
      </p>
      <p>{decodeHtml(description)}</p>
      <a href={url} target="_blank" rel="noopener noreferrer">Read more</a>
    </div>
  );
};

export default NewsItem;
