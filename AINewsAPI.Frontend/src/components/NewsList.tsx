import React, { useState, useEffect } from 'react';
import NewsItem from './NewsItem';

interface NewsArticle {
  id: number;
  title: string;
  description: string;
  url: string;
  publishedAt: string;
}

const NewsList: React.FC = () => {
  const [news, setNews] = useState<NewsArticle[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchNews = async () => {
      try {
        const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || '';
        console.log('VITE_API_BASE_URL:', apiBaseUrl);
        const response = await fetch(`${apiBaseUrl}/api/news`);
        if (!response.ok) {
          throw new Error('Failed to fetch news');
        }
        const data = await response.json();
        setNews(data);
        setLoading(false);
      } catch (err) {
        setError('Error fetching news. Please try again later.');
        setLoading(false);
      }
    };

    fetchNews();
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;

  return (
    <div className="news-list">
      {news.map((article) => (
        <NewsItem
          key={article.id}
          title={article.title}
          description={article.description}
          url={article.url}
          publishedAt={article.publishedAt}
        />
      ))}
    </div>
  );
};

export default NewsList;
