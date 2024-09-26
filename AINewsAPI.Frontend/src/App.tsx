import React, { useState, useEffect } from 'react';
import { Container, Typography, Card, CardContent, Grid, CircularProgress, Button, IconButton } from '@mui/material';
import FavoriteIcon from '@mui/icons-material/Favorite';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import axios from 'axios';
import EmailShareModal from './components/EmailShareModal';

interface NewsItem {
  id: string;
  title: string;
  description: string;
  url: string;
  publishedAt: string;
}

const App: React.FC = () => {
  const [news, setNews] = useState<NewsItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [shareModalOpen, setShareModalOpen] = useState(false);
  const [selectedNewsItem, setSelectedNewsItem] = useState<NewsItem | null>(null);
  const [favorites, setFavorites] = useState<string[]>([]);

  useEffect(() => {
    const fetchNews = async () => {
      try {
        const response = await axios.get<NewsItem[]>('http://localhost:5000/api/news');
        setNews(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching news:', error);
        setLoading(false);
      }
    };

    fetchNews();
    loadFavorites();
  }, []);

  const loadFavorites = () => {
    const storedFavorites = localStorage.getItem('favorites');
    if (storedFavorites) {
      setFavorites(JSON.parse(storedFavorites));
    }
  };

  const handleShareClick = (item: NewsItem) => {
    setSelectedNewsItem(item);
    setShareModalOpen(true);
  };

  const handleFavoriteToggle = (itemId: string) => {
    const newFavorites = favorites.includes(itemId)
      ? favorites.filter(id => id !== itemId)
      : [...favorites, itemId];
    
    setFavorites(newFavorites);
    localStorage.setItem('favorites', JSON.stringify(newFavorites));
  };

  return (
    <Container maxWidth="md">
      <Typography variant="h2" component="h1" gutterBottom>
        AI News
      </Typography>
      {loading ? (
        <CircularProgress />
      ) : (
        <Grid container spacing={3}>
          {news.map((item) => (
            <Grid item xs={12} key={item.id}>
              <Card>
                <CardContent>
                  <Typography variant="h5" component="h2">
                    {item.title}
                  </Typography>
                  <Typography color="textSecondary" gutterBottom>
                    {new Date(item.publishedAt).toLocaleString()}
                  </Typography>
                  <Typography variant="body2" component="p">
                    {item.description}
                  </Typography>
                  <Typography variant="body2" component="p">
                    <a href={item.url} target="_blank" rel="noopener noreferrer">
                      Read more
                    </a>
                  </Typography>
                  <Button
                    variant="contained"
                    color="primary"
                    onClick={() => handleShareClick(item)}
                    sx={{ mt: 2, mr: 1 }}
                  >
                    Share via Email
                  </Button>
                  <IconButton
                    color="primary"
                    onClick={() => handleFavoriteToggle(item.id)}
                    aria-label={favorites.includes(item.id) ? "Remove from favorites" : "Add to favorites"}
                  >
                    {favorites.includes(item.id) ? <FavoriteIcon /> : <FavoriteBorderIcon />}
                  </IconButton>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
      {selectedNewsItem && (
        <EmailShareModal
          open={shareModalOpen}
          onClose={() => setShareModalOpen(false)}
          newsItem={selectedNewsItem}
        />
      )}
    </Container>
  );
};

export default App;
