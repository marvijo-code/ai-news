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
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

  if (diffDays === 0) {
    return 'Today';
  } else if (diffDays === 1) {
    return 'Yesterday';
  } else if (diffDays < 7) {
    return `${diffDays} days ago`;
  } else {
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
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
